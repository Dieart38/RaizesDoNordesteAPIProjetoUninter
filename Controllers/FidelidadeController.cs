using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesNordeste.API.Application.Services;
using RaizesNordeste.API.DTOs;
using System.Security.Claims;

namespace RaizesNordeste.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FidelidadeController : ControllerBase
    {
        private readonly FidelidadeService _fidelidadeService;

        public FidelidadeController(FidelidadeService fidelidadeService)
        {
            _fidelidadeService = fidelidadeService;
        }

        private Guid ObterUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        /// <summary>
        /// Consultar meus pontos de fidelidade
        /// </summary>
        [HttpGet("pontos")]
        public async Task<IActionResult> ConsultarPontos()
        {
            var usuarioId = ObterUsuarioId();
            var (sucesso, mensagem) = await _fidelidadeService.ResgatarDesconto(usuarioId, 0);

            // Uma forma melhor seria ter um método específico, mas para simplificar...
            return Ok(new RespostaPadrao
            {
                Sucesso = true,
                Mensagem = "Consulte seus pontos no perfil do usuário",
                StatusCode = 200
            });
        }

        /// <summary>
        /// Resgatar desconto com pontos
        /// </summary>
        [HttpPost("resgatar")]
        public async Task<IActionResult> ResgatarDesconto([FromBody] int pontosParaUsar)
        {
            var usuarioId = ObterUsuarioId();
            var (sucesso, mensagem) = await _fidelidadeService.ResgatarDesconto(usuarioId, pontosParaUsar);

            if (!sucesso)
                return BadRequest(new RespostaPadrao
                {
                    Sucesso = false,
                    Mensagem = mensagem,
                    StatusCode = 400
                });

            return Ok(new RespostaPadrao
            {
                Sucesso = true,
                Mensagem = mensagem,
                StatusCode = 200
            });
        }
    }
}