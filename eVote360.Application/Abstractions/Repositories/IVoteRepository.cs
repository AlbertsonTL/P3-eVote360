using eVote360.Domain.Entities.Vote;

namespace eVote360.Application.Abstractions.Repositories;

public interface IVoteRepository : IGenericRepository<Vote>
{
    Task<bool> HasVotedInElectionAsync(int citizenId, int electionId);
    Task<IEnumerable<Vote>> GetVotesByElectionIdAsync(int electionId);
    Task<IEnumerable<int>> GetVotedPositionsAsync(int citizenId, int electionId);
    Task<Vote?> GetVoteByCitizenAndElectionAsync(int citizenId, int electionId);
}
