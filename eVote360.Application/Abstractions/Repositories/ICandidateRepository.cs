using eVote360.Domain.Entities.Candidate;

namespace eVote360.Application.Abstractions.Repositories;

public interface ICandidateRepository : IGenericRepository<Candidate>
{
    Task<IEnumerable<Candidate>> GetAllWithDetailsAsync();
    Task<IEnumerable<Candidate>> GetByPartyIdAsync(int partyId);
    Task<IEnumerable<Candidate>> GetByPositionIdAsync(int positionId);
}
