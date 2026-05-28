using RaizesNordeste.API.Domain.Enums;

namespace RaizesNordeste.API.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Cpf { get; set; }
        public PerfilUsuario Perfil { get; set; } = PerfilUsuario.CLIENTE;
        public bool ConsentimentoLGPD { get; set; }
        public DateTime? DataConsentimento { get; set; }
        public string SenhaHash { get; set; } = string.Empty;
        public int PontosFidelidade { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Relacionamentos
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}