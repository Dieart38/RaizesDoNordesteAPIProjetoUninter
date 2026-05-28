
namespace RaizesNordeste.API.DTOs
{
    public class RegistroDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Cpf { get; set; }
        public string Senha { get; set; } = string.Empty;
        public bool ConsentimentoLGPD { get; set; }
    }
}