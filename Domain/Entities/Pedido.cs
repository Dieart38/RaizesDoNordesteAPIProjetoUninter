using RaizesNordeste.API.Domain.Enums;

namespace RaizesNordeste.API.Domain.Entities
{
    public class Pedido
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public StatusPedido Status { get; set; } = StatusPedido.AGUARDANDO_PAGAMENTO;
        public CanalOrigem CanalOrigem { get; set; }
        public decimal ValorTotal { get; set; }

        // Chaves estrangeiras
        public Guid UsuarioId { get; set; }
        public Guid UnidadeId { get; set; }

        // Relacionamentos
        public Usuario Usuario { get; set; } = null!;
        public Unidade Unidade { get; set; } = null!;
        public ICollection<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
        public PagamentoMock? Pagamento { get; set; }
    }
}