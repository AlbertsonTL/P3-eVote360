using Microsoft.EntityFrameworkCore;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Domain.Entities.Candidate;
using eVote360.Infrastructure.Persistence;
using eVote360.Infrastructure.Repositories.Base;

namespace eVote360.Infrastructure.Repositories;

public class CandidateRepository : GenericRepository<Candidate>, ICandidateRepository
{
    public CandidateRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Candidate>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(c => c.Party)
            .Include(c => c.Position)
            .OrderBy(c => c.Party.Nombre)
            .ThenBy(c => c.Apellido)
            .ToListAsync();
    }

    public async Task<IEnumerable<Candidate>> GetByPartyIdAsync(int partyId)
    {
        return await _dbSet
            .Include(c => c.Party)
            .Include(c => c.Position)
            .Where(c => c.PartyId == partyId)
            .OrderBy(c => c.Apellido)
            .ToListAsync();
    }

    public async Task<IEnumerable<Candidate>> GetByPositionIdAsync(int positionId)
    {
        return await _dbSet
            .Include(c => c.Party)
            .Where(c => c.PositionId == positionId && c.IsActive)
            .ToListAsync();
    }
}
