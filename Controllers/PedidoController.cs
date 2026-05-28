using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesNordeste.API.Application.Services;
using RaizesNordeste.API.Domain.Entities;
using RaizesNordeste.API.Domain.Enums;
using RaizesNordeste.API.DTOs;
using System.Security.Claims;

namespace RaizesNordeste.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PedidoController : ControllerBase
    {
        private readonly PedidoService _pedidoService;
        private readonly PagamentoMockService _pagamentoService;
        private readonly FidelidadeService _fidelidadeService;

        public PedidoController(
            PedidoService pedidoService,
            PagamentoMockService pagamentoService,
            FidelidadeService fidelidadeService)
        {
            _pedidoService = pedidoService;
            _pagamentoService = pagamentoService;
            _fidelidadeService = fidelidadeService;
        }

        private Guid ObterUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }

        private string ObterPerfilUsuario()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? "CLIENTE";
        }

        /// <summary>
        /// Criar um novo pedido
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDto dto)
        {
            var usuarioId = ObterUsuarioId();
            var pedido = await _pedidoService.CriarPedido(usuarioId, dto);

            if (pedido == null)
                return Conflict(new RespostaPadrao
                {
                    Sucesso = false,
                    Mensagem = "Não foi possível criar o pedido. Verifique estoque ou disponibilidade dos produtos.",
                    StatusCode = 409
                });

            return CreatedAtAction(nameof(ObterPedido), new { id = pedido.Id }, new RespostaPadrao
            {
                Sucesso = true,
                Mensagem = "Pedido criado com sucesso. Aguardando pagamento.",
                Dados = pedido,
                StatusCode = 201
            });
        }

        /// <summary>
        /// Obter detalhes de um pedido
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPedido(Guid id)
        {
            var pedido = await _pedidoService.ObterPedido(id);
            if (pedido == null)
                return NotFound(new RespostaPadrao
                {
                    Sucesso = false,
                    Mensagem = "Pedido não encontrado",
                    StatusCode = 404
                });

            return Ok(new RespostaPadrao
            {
                Sucesso = true,
                Dados = pedido,
                StatusCode = 200
            });
        }

        /// <summary>
        /// Listar meus pedidos (opcional filtrar por canal)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ListarMeusPedidos([FromQuery] string? canal = null)
        {
            var usuarioId = ObterUsuarioId();
            CanalOrigem? canalEnum = null;

            if (!string.IsNullOrEmpty(canal) && Enum.TryParse<CanalOrigem>(canal, true, out var parsed))
                canalEnum = parsed;

            var pedidos = await _pedidoService.ListarPedidosDoUsuario(usuarioId, canalEnum);

            return Ok(new RespostaPadrao
            {
                Sucesso = true,
                Dados = pedidos,
                StatusCode = 200
            });
        }

        /// <summary>
        /// Atualizar status do pedido (Cozinha/Gerente)
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(Guid id, [FromBody] AtualizarStatusDto dto)
        {
            var perfil = ObterPerfilUsuario();
            var resultado = await _pedidoService.AtualizarStatus(id, dto.Status, perfil);

            if (!resultado)
                return StatusCode(403, new RespostaPadrao
                {
                    Sucesso = false,
                    Mensagem = "Você não tem permissão para alterar este status ou a transição não é permitida",
                    StatusCode = 403
                });

            return Ok(new RespostaPadrao
            {
                Sucesso = true,
                Mensagem = $"Status do pedido atualizado para {dto.Status}",
                StatusCode = 200
            });
        }

        /// <summary>
        /// Processar pagamento mock
        /// </summary>
        [HttpPost("{id}/pagamento")]
        public async Task<IActionResult> ProcessarPagamento(Guid id, [FromBody] PagamentoDto dto)
        {
            var (aprovado, transacaoId, mensagem) = await _pagamentoService.ProcessarPagamento(id, dto);

            if (aprovado)
            {
                var pedido = await _pedidoService.ObterPedido(id);
                if (pedido != null)
                {
                    var pontos = await _fidelidadeService.AcumularPontos(pedido.UsuarioId, pedido.ValorTotal);
                    return Ok(new RespostaPadrao
                    {
                        Sucesso = true,
                        Mensagem = $"{mensagem} Você ganhou {pontos} pontos de fidelidade!",
                        Dados = new { TransacaoId = transacaoId, PontosGanhos = pontos },
                        StatusCode = 200
                    });
                }
            }

            if (!aprovado)
                return BadRequest(new RespostaPadrao
                {
                    Sucesso = false,
                    Mensagem = mensagem,
                    Dados = new { TransacaoId = transacaoId },
                    StatusCode = 400
                });

            return Ok(new RespostaPadrao
            {
                Sucesso = true,
                Mensagem = mensagem,
                Dados = new { TransacaoId = transacaoId },
                StatusCode = 200
            });
        }
    }
}