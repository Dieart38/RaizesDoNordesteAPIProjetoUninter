using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaizesNordeste.API.Application.Services;
using RaizesNordeste.API.DTOs;

namespace RaizesNordeste.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnidadeController : ControllerBase
    {
        private readonly UnidadeService _unidadeService;
        private readonly ProdutoService _produtoService;

        public UnidadeController(UnidadeService unidadeService, ProdutoService produtoService)
        {
            _unidadeService = unidadeService;
            _produtoService = produtoService;
        }

        /// <summary>
        /// Listar todas as unidades ativas
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ListarUnidades()
        {
            var unidades = await _unidadeService.ListarUnidadesAtivas();
            return Ok(new RespostaPadrao
            {
                Sucesso = true,
                Dados = unidades,
                StatusCode = 200
            });
        }

        /// <summary>
        /// Obter cardápio de uma unidade específica
        /// </summary>
        [HttpGet("{unidadeId}/cardapio")]
        [AllowAnonymous]
        public async Task<IActionResult> ObterCardapio(Guid unidadeId)
        {
            var produtos = await _produtoService.ListarProdutosDisponiveis(unidadeId);
            return Ok(new RespostaPadrao
            {
                Sucesso = true,
                Dados = produtos,
                StatusCode = 200
            });
        }
    }
}