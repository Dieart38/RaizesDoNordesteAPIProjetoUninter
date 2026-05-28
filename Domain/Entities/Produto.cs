namespace RaizesNordeste.API.Domain.Entities
{
    public class Produto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public bool IsSazonal { get; set; }
        public DateTime? InicioSazonalidade { get; set; }
        public DateTime? FimSazonalidade { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Relacionamentos
        public ICollection<ItemPedido> ItensPedido { get; set; } = new List<ItemPedido>();
        public ICollection<Estoque> Estoques { get; set; } = new List<Estoque>();
    }
}