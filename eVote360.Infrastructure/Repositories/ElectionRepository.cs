using Microsoft.EntityFrameworkCore;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Domain.Entities.Election;
using eVote360.Domain.Enums;
using eVote360.Infrastructure.Persistence;
using eVote360.Infrastructure.Repositories.Base;

namespace eVote360.Infrastructure.Repositories;

public class ElectionRepository : GenericRepository<Election>, IElectionRepository
{
    public ElectionRepository(AppDbContext context) : base(context) { }

    public async Task<Election?> GetActiveElectionAsync()
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Estado == ElectionState.EnProceso);
    }

    public async Task<IEnumerable<Election>> GetElectionsByYearAsync(int year)
    {
        return await _dbSet.Where(e => e.FechaRealizacion.Year == year).ToListAsync();
    }
}
