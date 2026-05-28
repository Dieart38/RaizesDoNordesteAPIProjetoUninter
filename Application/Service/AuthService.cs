using Microsoft.IdentityModel.Tokens;
using RaizesNordeste.API.Domain.Entities;
using RaizesNordeste.API.Domain.Enums;
using RaizesNordeste.API.DTOs;
using RaizesNordeste.API.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RaizesNordeste.API.Application.Services
{
    public class AuthService
    {
        private readonly IRepository<Usuario> _usuarioRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IRepository<Usuario> usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public async Task<Usuario?> Registrar(RegistroDto dto)
        {
            // Verificar se email já existe
            var existe = await _usuarioRepository.ExistsAsync(u => u.Email == dto.Email);
            if (existe) return null;

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Cpf = dto.Cpf,
                ConsentimentoLGPD = dto.ConsentimentoLGPD,
                DataConsentimento = dto.ConsentimentoLGPD ? DateTime.UtcNow : null,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                Perfil = PerfilUsuario.CLIENTE,
                PontosFidelidade = 0
            };

            return await _usuarioRepository.AddAsync(usuario);
        }

        public async Task<(bool Sucesso, string Token, Usuario? Usuario)> Login(LoginDto dto)
        {
            var usuario = (await _usuarioRepository.FindAsync(u => u.Email == dto.Email)).FirstOrDefault();

            if (usuario == null)
                return (false, string.Empty, null);

            if (!BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
                return (false, string.Empty, null);

            var token = GerarTokenJwt(usuario);
            return (true, token, usuario);
        }

        private string GerarTokenJwt(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),
                new Claim("Nome", usuario.Nome)
            };

            var key = new SymmetricSecurityKey(secretKey);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpirationHours"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}