using Microsoft.EntityFrameworkCore;
using RaizesNordeste.API.Domain.Entities;
using RaizesNordeste.API.Domain.Enums;
using RaizesNordeste.API.DTOs;
using RaizesNordeste.API.Infrastructure;

namespace RaizesNordeste.API.Application.Services
{
    public class PedidoService
    {
        private readonly IRepository<Pedido> _pedidoRepository;
        private readonly IRepository<Produto> _produtoRepository;
        private readonly IRepository<Estoque> _estoqueRepository;
        private readonly IRepository<Usuario> _usuarioRepository;
        private readonly AppDbContext _context;

        public PedidoService(
            IRepository<Pedido> pedidoRepository,
            IRepository<Produto> produtoRepository,
            IRepository<Estoque> estoqueRepository,
            IRepository<Usuario> usuarioRepository,
            AppDbContext context)
        {
            _pedidoRepository = pedidoRepository;
            _produtoRepository = produtoRepository;
            _estoqueRepository = estoqueRepository;
            _usuarioRepository = usuarioRepository;
            _context = context;
        }

        public async Task<Pedido?> CriarPedido(Guid usuarioId, CriarPedidoDto dto)
        {
            // Validar se o usuário existe
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null) return null;

            // Validar estoque de todos os itens antes de criar o pedido
            foreach (var itemDto in dto.Itens)
            {
                var produto = await _produtoRepository.GetByIdAsync(itemDto.ProdutoId);
                if (produto == null) return null;

                // Verificar sazonalidade do produto
                if (produto.IsSazonal)
                {
                    var hoje = DateTime.UtcNow;
                    if (produto.InicioSazonalidade.HasValue && produto.FimSazonalidade.HasValue)
                    {
                        if (hoje < produto.InicioSazonalidade || hoje > produto.FimSazonalidade)
                            return null; // Produto fora da temporada
                    }
                }

                // Verificar estoque
                var estoque = (await _estoqueRepository.FindAsync(e =>
                    e.UnidadeId == dto.UnidadeId && e.ProdutoId == itemDto.ProdutoId)).FirstOrDefault();

                if (estoque == null || estoque.Quantidade < itemDto.Quantidade)
                    return null; // Estoque insuficiente
            }

            // Criar o pedido
            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                UnidadeId = dto.UnidadeId,
                CanalOrigem = dto.CanalPedido,
                Status = StatusPedido.AGUARDANDO_PAGAMENTO,
                DataCriacao = DateTime.UtcNow,
                ValorTotal = 0
            };

            await _pedidoRepository.AddAsync(pedido);

            // Adicionar os itens e calcular total
            decimal total = 0;
            foreach (var itemDto in dto.Itens)
            {
                var produto = await _produtoRepository.GetByIdAsync(itemDto.ProdutoId);
                var item = new ItemPedido
                {
                    PedidoId = pedido.Id,
                    ProdutoId = itemDto.ProdutoId,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = produto!.Preco
                };

                _context.ItensPedido.Add(item);
                total += produto.Preco * itemDto.Quantidade;
            }

            pedido.ValorTotal = total;
            await _pedidoRepository.UpdateAsync(pedido);

            return pedido;
        }

        public async Task<bool> AtualizarStatus(Guid pedidoId, StatusPedido novoStatus, string perfilUsuario)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null) return false;

            // Regras de autorização por perfil
            if (perfilUsuario == "CLIENTE")
            {
                // Cliente só pode cancelar pedidos que estão aguardando pagamento
                if (novoStatus == StatusPedido.CANCELADO && pedido.Status == StatusPedido.AGUARDANDO_PAGAMENTO)
                {
                    pedido.Status = novoStatus;
                    await _pedidoRepository.UpdateAsync(pedido);
                    return true;
                }
                return false;
            }

            if (perfilUsuario == "COZINHA")
            {
                // Cozinha pode alterar: RECEBIDO -> EM_PREPARO -> PRONTO
                if ((pedido.Status == StatusPedido.RECEBIDO && novoStatus == StatusPedido.EM_PREPARO) ||
                    (pedido.Status == StatusPedido.EM_PREPARO && novoStatus == StatusPedido.PRONTO))
                {
                    pedido.Status = novoStatus;
                    await _pedidoRepository.UpdateAsync(pedido);
                    return true;
                }
                return false;
            }

            if (perfilUsuario == "GERENTE" || perfilUsuario == "ADMIN")
            {
                // Gerente pode alterar qualquer status
                pedido.Status = novoStatus;
                await _pedidoRepository.UpdateAsync(pedido);
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Pedido>> ListarPedidosDoUsuario(Guid usuarioId, CanalOrigem? canal = null)
        {
            var query = (await _pedidoRepository.FindAsync(p => p.UsuarioId == usuarioId)).AsQueryable();

            if (canal.HasValue)
                query = query.Where(p => p.CanalOrigem == canal.Value);

            return query.OrderByDescending(p => p.DataCriacao).ToList();
        }

        public async Task<Pedido?> ObterPedido(Guid pedidoId)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido != null)
            {
                // Carregar os itens e produtos relacionados
                await _context.Entry(pedido)
                    .Collection(p => p.Itens)
                    .Query()
                    .Include(i => i.Produto)
                    .LoadAsync();
            }
            return pedido;
        }

        public async Task<bool> ConfirmarPagamento(Guid pedidoId)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null || pedido.Status != StatusPedido.AGUARDANDO_PAGAMENTO)
                return false;

            pedido.Status = StatusPedido.RECEBIDO;
            await _pedidoRepository.UpdateAsync(pedido);

            // Baixar o estoque após confirmação do pagamento
            foreach (var item in pedido.Itens)
            {
                var estoque = (await _estoqueRepository.FindAsync(e =>
                    e.UnidadeId == pedido.UnidadeId && e.ProdutoId == item.ProdutoId)).FirstOrDefault();

                if (estoque != null)
                {
                    estoque.Quantidade -= item.Quantidade;
                    await _estoqueRepository.UpdateAsync(estoque);
                }
            }

            return true;
        }
    }
}