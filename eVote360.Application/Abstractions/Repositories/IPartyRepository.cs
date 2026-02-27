using eVote360.Domain.Entities.Party;

namespace eVote360.Application.Abstractions.Repositories;

public interface IPartyRepository : IGenericRepository<Party>
{
    Task<Party?> GetBySiglasAsync(string siglas);
    Task<IEnumerable<Party>> GetActivePartiesAsync();
}
