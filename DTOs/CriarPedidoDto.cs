using RaizesNordeste.API.Domain.Enums;

namespace RaizesNordeste.API.DTOs
{
    public class CriarPedidoDto
    {
        public Guid UnidadeId { get; set; }
        public CanalOrigem CanalPedido { get; set; }
        public List<ItemPedidoDto> Itens { get; set; } = new();
    }

    public class ItemPedidoDto
    {
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}