using eVote360.Domain.Entities.Alliance;

namespace eVote360.Application.Abstractions.Repositories;

public interface IAllianceRepository : IGenericRepository<Alliance>
{
    Task<IEnumerable<Alliance>> GetByPartyIdAsync(int partyId);
    Task<Alliance?> GetPendingBetweenPartiesAsync(int partyAId, int partyBId);
    Task<IEnumerable<Alliance>> GetAllWithDetailsAsync();
    Task<bool> HasActiveAllianceAsync(int partyAId, int partyBId);
}
