using Microsoft.EntityFrameworkCore;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Domain.Entities.Position;
using eVote360.Infrastructure.Persistence;
using eVote360.Infrastructure.Repositories.Base;

namespace eVote360.Infrastructure.Repositories;

public class PositionRepository : GenericRepository<Position>, IPositionRepository
{
    public PositionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Position>> GetActivePositionsAsync()
    {
        return await _dbSet.Where(p => p.IsActive).ToListAsync();
    }
}
