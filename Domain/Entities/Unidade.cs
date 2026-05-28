namespace RaizesNordeste.API.Domain.Entities
{
    public class Unidade
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public bool Ativa { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Relacionamentos
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public ICollection<Estoque> Estoques { get; set; } = new List<Estoque>();
    }
}