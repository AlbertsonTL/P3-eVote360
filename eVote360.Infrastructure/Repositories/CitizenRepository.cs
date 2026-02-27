using Microsoft.EntityFrameworkCore;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Domain.Entities.Citizen;
using eVote360.Infrastructure.Persistence;
using eVote360.Infrastructure.Repositories.Base;

namespace eVote360.Infrastructure.Repositories;

public class CitizenRepository : GenericRepository<Citizen>, ICitizenRepository
{
    public CitizenRepository(AppDbContext context) : base(context) { }

    public async Task<Citizen?> GetByNationalIdAsync(string nationalId)
    {
        // Cargar en memoria primero para poder acceder al Value Object
        var citizens = await _dbSet.ToListAsync();
        return citizens.FirstOrDefault(c => c.NumeroDocumento.Value == nationalId);
    }

    public async Task<bool> ExistsDifferentCitizenWithNationalIdAsync(int citizenId, string nationalId)
    {
        var citizens = await _dbSet.ToListAsync();
        return citizens.Any(c => c.NumeroDocumento.Value == nationalId && c.Id != citizenId);
    }
}
