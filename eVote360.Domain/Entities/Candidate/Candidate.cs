using eVote360.Domain.Base;

namespace eVote360.Domain.Entities.Candidate;

public class Candidate : AuditableEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string FotoPath { get; set; } = string.Empty;
    public int PartyId { get; set; }
    public Party.Party Party { get; set; } = null!;
    public int? PositionId { get; set; }
    public Position.Position? Position { get; set; }
}
