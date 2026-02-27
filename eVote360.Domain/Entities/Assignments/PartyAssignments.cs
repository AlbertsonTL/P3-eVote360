using eVote360.Domain.Base;

namespace eVote360.Domain.Entities.Assignments;

public class PartyAssignments : BaseEntity
{
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public int PartyId { get; set; }
    public Party.Party Party { get; set; } = null!;
}
