using eVote360.Domain.Base;
using eVote360.Domain.ValueObjects;

namespace eVote360.Domain.Entities.Citizen;

public class Citizen : AuditableEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public EmailAddress Email { get; set; } = null!;
    public NationalId NumeroDocumento { get; set; } = null!;
}
