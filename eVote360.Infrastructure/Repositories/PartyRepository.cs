using Microsoft.EntityFrameworkCore;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Domain.Entities.Party;
using eVote360.Infrastructure.Persistence;
using eVote360.Infrastructure.Repositories.Base;

namespace eVote360.Infrastructure.Repositories;

public class PartyRepository : GenericRepository<Party>, IPartyRepository
{
    public PartyRepository(AppDbContext context) : base(context) { }

    public async Task<Party?> GetBySiglasAsync(string siglas)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Siglas == siglas);
    }

    public async Task<IEnumerable<Party>> GetActivePartiesAsync()
    {
        return await _dbSet.Where(p => p.IsActive).ToListAsync();
    }
}
