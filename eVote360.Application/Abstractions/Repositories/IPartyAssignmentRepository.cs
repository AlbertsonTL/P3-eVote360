using eVote360.Domain.Entities.Assignments;

namespace eVote360.Application.Abstractions.Repositories;

public interface IPartyAssignmentRepository : IGenericRepository<PartyAssignments>
{
    Task<PartyAssignments?> GetByUsuarioIdAsync(int usuarioId);
    Task<IEnumerable<PartyAssignments>> GetAllWithDetailsAsync();
}
