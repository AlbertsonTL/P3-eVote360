using eVote360.Domain.Entities.Election;

namespace eVote360.Application.Abstractions.Policies;

public interface IElectionReadService
{
    Task<Election?> GetActiveElectionAsync();
}
