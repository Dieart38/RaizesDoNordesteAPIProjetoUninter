using RaizesNordeste.API.Domain.Entities;
using RaizesNordeste.API.Infrastructure;

namespace RaizesNordeste.API.Application.Services
{
    public class UnidadeService
    {
        private readonly IRepository<Unidade> _unidadeRepository;

        public UnidadeService(IRepository<Unidade> unidadeRepository)
        {
            _unidadeRepository = unidadeRepository;
        }

        public async Task<IEnumerable<Unidade>> ListarUnidadesAtivas()
        {
            return await _unidadeRepository.FindAsync(u => u.Ativa);
        }

        public async Task<Unidade?> ObterUnidade(Guid id)
        {
            return await _unidadeRepository.GetByIdAsync(id);
        }
    }
}
