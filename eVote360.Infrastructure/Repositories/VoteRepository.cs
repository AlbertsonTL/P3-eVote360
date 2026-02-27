using Microsoft.EntityFrameworkCore;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Domain.Entities.Vote;
using eVote360.Infrastructure.Persistence;
using eVote360.Infrastructure.Repositories.Base;

namespace eVote360.Infrastructure.Repositories;

public class VoteRepository : GenericRepository<Vote>, IVoteRepository
{
    public VoteRepository(AppDbContext context) : base(context) { }

    public async Task<bool> HasVotedInElectionAsync(int citizenId, int electionId)
    {
        return await _dbSet.AnyAsync(v => v.CitizenId == citizenId && v.ElectionId == electionId);
    }

    public async Task<IEnumerable<Vote>> GetVotesByElectionIdAsync(int electionId)
    {
        return await _dbSet.Include(v => v.VoteItems)
                          .Where(v => v.ElectionId == electionId)
                          .ToListAsync();
    }

    public async Task<IEnumerable<int>> GetVotedPositionsAsync(int citizenId, int electionId)
    {
        var votes = await _dbSet
            .Include(v => v.VoteItems)
            .Where(v => v.CitizenId == citizenId && v.ElectionId == electionId)
            .ToListAsync();

        return votes.SelectMany(v => v.VoteItems)
                    .Where(vi => vi.PositionId > 0)
                    .Select(vi => vi.PositionId)
                    .Distinct()
                    .ToList();
    }

    /// <summary>Eficientemente obtiene el voto de un ciudadano para una elección específica.</summary>
    public async Task<Vote?> GetVoteByCitizenAndElectionAsync(int citizenId, int electionId)
    {
        return await _dbSet
            .Include(v => v.VoteItems)
                .ThenInclude(vi => vi.Position)
            .Include(v => v.VoteItems)
                .ThenInclude(vi => vi.Candidate)
            .Include(v => v.VoteItems)
                .ThenInclude(vi => vi.Party)
            .FirstOrDefaultAsync(v => v.CitizenId == citizenId && v.ElectionId == electionId);
    }
}
