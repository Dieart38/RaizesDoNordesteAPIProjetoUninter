using RaizesNordeste.API.Domain.Entities;
using RaizesNordeste.API.Infrastructure;

namespace RaizesNordeste.API.Application.Services
{
    public class EstoqueService
    {
        private readonly IRepository<Estoque> _estoqueRepository;
        private readonly IRepository<Produto> _produtoRepository;
        private readonly IRepository<Unidade> _unidadeRepository;

        public EstoqueService(
            IRepository<Estoque> estoqueRepository,
            IRepository<Produto> produtoRepository,
            IRepository<Unidade> unidadeRepository)
        {
            _estoqueRepository = estoqueRepository;
            _produtoRepository = produtoRepository;
            _unidadeRepository = unidadeRepository;
        }

        public async Task<int> ConsultarEstoque(Guid unidadeId, Guid produtoId)
        {
            var estoque = (await _estoqueRepository.FindAsync(e =>
                e.UnidadeId == unidadeId && e.ProdutoId == produtoId)).FirstOrDefault();

            return estoque?.Quantidade ?? 0;
        }

        public async Task<bool> MovimentarEstoque(Guid unidadeId, Guid produtoId, int quantidade, string tipo, Guid usuarioId)
        {
            var produto = await _produtoRepository.GetByIdAsync(produtoId);
            var unidade = await _unidadeRepository.GetByIdAsync(unidadeId);

            if (produto == null || unidade == null)
                return false;

            var estoque = (await _estoqueRepository.FindAsync(e =>
                e.UnidadeId == unidadeId && e.ProdutoId == produtoId)).FirstOrDefault();

            if (estoque == null)
            {
                // Criar novo registro de estoque
                estoque = new Estoque
                {
                    UnidadeId = unidadeId,
                    ProdutoId = produtoId,
                    Quantidade = 0
                };
                await _estoqueRepository.AddAsync(estoque);
            }

            if (tipo == "ENTRADA")
                estoque.Quantidade += quantidade;
            else if (tipo == "SAIDA")
            {
                if (estoque.Quantidade < quantidade)
                    return false; // Estoque insuficiente
                estoque.Quantidade -= quantidade;
            }
            else
                return false;

            await _estoqueRepository.UpdateAsync(estoque);
            return true;
        }
    }
}