namespace RaizesNordeste.API.Domain.Entities
{
    public class Estoque
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Quantidade { get; set; }

        // Chaves estrangeiras
        public Guid UnidadeId { get; set; }
        public Guid ProdutoId { get; set; }

        // Relacionamentos
        public Unidade Unidade { get; set; } = null!;
        public Produto Produto { get; set; } = null!;
    }
}