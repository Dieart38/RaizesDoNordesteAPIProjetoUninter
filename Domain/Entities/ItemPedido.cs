namespace RaizesNordeste.API.Domain.Entities
{
    public class ItemPedido
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }

        // Chaves estrangeiras
        public Guid PedidoId { get; set; }
        public Guid ProdutoId { get; set; }

        // Relacionamentos
        public Pedido Pedido { get; set; } = null!;
        public Produto Produto { get; set; } = null!;
    }
}