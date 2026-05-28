using RaizesNordeste.API.Domain.Entities;
using RaizesNordeste.API.Infrastructure;

namespace RaizesNordeste.API.Application.Services
{
    public class FidelidadeService
    {
        private readonly IRepository<Usuario> _usuarioRepository;

        public FidelidadeService(IRepository<Usuario> usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // Regra: 1 ponto a cada R$10 gastos
        public async Task<int> AcumularPontos(Guid usuarioId, decimal valorGasto)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null) return 0;

            int pontos = (int)(valorGasto / 10);
            usuario.PontosFidelidade += pontos;
            await _usuarioRepository.UpdateAsync(usuario);

            return pontos;
        }

        public async Task<(bool Sucesso, string Mensagem)> ResgatarDesconto(Guid usuarioId, int pontosParaUsar)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
                return (false, "Usuário não encontrado");

            if (usuario.PontosFidelidade < pontosParaUsar)
                return (false, $"Pontos insuficientes. Você tem {usuario.PontosFidelidade} pontos");

            usuario.PontosFidelidade -= pontosParaUsar;
            await _usuarioRepository.UpdateAsync(usuario);

            decimal desconto = pontosParaUsar * 0.25m; // R$0,25 de desconto por ponto
            return (true, $"Desconto de R${desconto:F2} aplicado. Pontos restantes: {usuario.PontosFidelidade}");
        }
    }
}