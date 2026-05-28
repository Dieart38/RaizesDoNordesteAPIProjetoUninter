using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesNordeste.API.Application.Services;
using RaizesNordeste.API.DTOs;

namespace RaizesNordeste.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registrar um novo usuário
        /// </summary>
        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<IActionResult> Registrar([FromBody] RegistroDto dto)
        {
            if (!dto.ConsentimentoLGPD)
                return BadRequest(new RespostaPadrao
                {
                    Sucesso = false,
                    Mensagem = "Você precisa aceitar os termos da LGPD para se cadastrar",
                    StatusCode = 400
                });

            var usuario = await _authService.Registrar(dto);
            if (usuario == null)
                return Conflict(new RespostaPadrao
                {
                    Sucesso = false,
                    Mensagem = "E-mail já cadastrado",
                    StatusCode = 409
                });

            return Ok(new RespostaPadrao
            {
                Sucesso = true,
                Mensagem = "Usuário cadastrado com sucesso",
                Dados = new { usuario.Id, usuario.Nome, usuario.Email },
                StatusCode = 200
            });
        }

        /// <summary>
        /// Fazer login e obter token JWT
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var (sucesso, token, usuario) = await _authService.Login(dto);

            if (!sucesso)
                return Unauthorized(new RespostaPadrao
                {
                    Sucesso = false,
                    Mensagem = "E-mail ou senha inválidos",
                    StatusCode = 401
                });

            return Ok(new RespostaPadrao
            {
                Sucesso = true,
                Mensagem = "Login realizado com sucesso",
                Dados = new
                {
                    Token = token,
                    Usuario = new
                    {
                        usuario!.Id,
                        usuario.Nome,
                        usuario.Email,
                        usuario.Perfil,
                        usuario.PontosFidelidade
                    }
                },
                StatusCode = 200
            });
        }
    }
}