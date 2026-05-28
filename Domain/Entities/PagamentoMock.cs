namespace RaizesNordeste.API.Domain.Entities
{
    public class PagamentoMock
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Status { get; set; } = string.Empty; // "APROVADO" ou "RECUSADO"
        public string TransacaoId { get; set; } = string.Empty;
        public string? Mensagem { get; set; }
        public DateTime Data { get; set; } = DateTime.UtcNow;

        // Chave estrangeira
        public Guid PedidoId { get; set; }

        // Relacionamento
        public Pedido Pedido { get; set; } = null!;
    }
}