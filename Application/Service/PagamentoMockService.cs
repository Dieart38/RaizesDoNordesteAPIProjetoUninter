using RaizesNordeste.API.Domain.Entities;
using RaizesNordeste.API.DTOs;
using RaizesNordeste.API.Infrastructure;

namespace RaizesNordeste.API.Application.Services
{
    public class PagamentoMockService
    {
        private readonly IRepository<Pedido> _pedidoRepository;
        private readonly IRepository<PagamentoMock> _pagamentoRepository;

        public PagamentoMockService(
            IRepository<Pedido> pedidoRepository,
            IRepository<PagamentoMock> pagamentoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _pagamentoRepository = pagamentoRepository;
        }

        public async Task<(bool Aprovado, string TransacaoId, string Mensagem)> ProcessarPagamento(Guid pedidoId, PagamentoDto dto)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(pedidoId);
            if (pedido == null)
                return (false, string.Empty, "Pedido não encontrado");

            if (pedido.Status != Domain.Enums.StatusPedido.AGUARDANDO_PAGAMENTO)
                return (false, string.Empty, "Pedido não está aguardando pagamento");

            // REGRA DE NEGÓCIO DO MOCK:
            // Cartões com número 4111111111111111 = aprovado
            // Cartões com número 5555555555554444 = recusado
            bool aprovado = false;
            string mensagem = "";
            string transacaoId = Guid.NewGuid().ToString();

            if (dto.CartaoMock?.Numero == "4111111111111111")
            {
                aprovado = true;
                mensagem = "Pagamento aprovado!";
            }
            else if (dto.CartaoMock?.Numero == "5555555555554444")
            {
                aprovado = false;
                mensagem = "Pagamento recusado. Saldo insuficiente.";
            }
            else
            {
                aprovado = false;
                mensagem = "Número de cartão inválido para teste. Use 4111111111111111 (aprovado) ou 5555555555554444 (recusado)";
            }

            // Registrar o pagamento mock
            var pagamento = new PagamentoMock
            {
                PedidoId = pedidoId,
                Status = aprovado ? "APROVADO" : "RECUSADO",
                TransacaoId = transacaoId,
                Mensagem = mensagem,
                Data = DateTime.UtcNow
            };

            await _pagamentoRepository.AddAsync(pagamento);

            // Atualizar status do pedido
            if (aprovado)
            {
                pedido.Status = Domain.Enums.StatusPedido.RECEBIDO;
                await _pedidoRepository.UpdateAsync(pedido);
            }
            else
            {
                pedido.Status = Domain.Enums.StatusPedido.PAGAMENTO_RECUSADO;
                await _pedidoRepository.UpdateAsync(pedido);
            }

            return (aprovado, transacaoId, mensagem);
        }
    }
}