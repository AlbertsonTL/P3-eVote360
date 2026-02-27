using Microsoft.EntityFrameworkCore;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Domain.Entities.Assignments;
using eVote360.Infrastructure.Persistence;
using eVote360.Infrastructure.Repositories.Base;

namespace eVote360.Infrastructure.Repositories;

public class PartyAssignmentRepository : GenericRepository<PartyAssignments>, IPartyAssignmentRepository
{
    public PartyAssignmentRepository(AppDbContext context) : base(context) { }

    public async Task<PartyAssignments?> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _dbSet.FirstOrDefaultAsync(pa => pa.UsuarioId == usuarioId);
    }

    public async Task<IEnumerable<PartyAssignments>> GetAllWithDetailsAsync()
    {
        return await _dbSet.Include(pa => pa.Usuario)
                          .Include(pa => pa.Party)
                          .ToListAsync();
    }
}
