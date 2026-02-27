using eVote360.Domain.Entities.Election;

namespace eVote360.Application.Abstractions.Repositories;

public interface IElectionRepository : IGenericRepository<Election>
{
    Task<Election?> GetActiveElectionAsync();
    Task<IEnumerable<Election>> GetElectionsByYearAsync(int year);
}
