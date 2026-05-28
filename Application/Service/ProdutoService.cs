using RaizesNordeste.API.Domain.Entities;
using RaizesNordeste.API.Infrastructure;

namespace RaizesNordeste.API.Application.Services
{
    public class ProdutoService
    {
        private readonly IRepository<Produto> _produtoRepository;
        private readonly IRepository<Estoque> _estoqueRepository;

        public ProdutoService(
            IRepository<Produto> produtoRepository,
            IRepository<Estoque> estoqueRepository)
        {
            _produtoRepository = produtoRepository;
            _estoqueRepository = estoqueRepository;
        }

        public async Task<IEnumerable<Produto>> ListarProdutosDisponiveis(Guid unidadeId)
        {
            // Buscar produtos que têm estoque > 0 na unidade
            var estoques = await _estoqueRepository.FindAsync(e => e.UnidadeId == unidadeId && e.Quantidade > 0);
            var produtoIds = estoques.Select(e => e.ProdutoId).ToList();

            var produtos = await _produtoRepository.FindAsync(p => produtoIds.Contains(p.Id));

            // Filtrar produtos sazonais (verificar se estão na temporada)
            var hoje = DateTime.UtcNow;
            return produtos.Where(p => !p.IsSazonal ||
                (p.InicioSazonalidade.HasValue && p.FimSazonalidade.HasValue &&
                 hoje >= p.InicioSazonalidade && hoje <= p.FimSazonalidade));
        }

        public async Task<Produto?> ObterProduto(Guid id)
        {
            return await _produtoRepository.GetByIdAsync(id);
        }
    }
}