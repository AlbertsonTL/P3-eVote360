using eVote360.Domain.Base;

namespace eVote360.Domain.Entities.Party;

public class Party : AuditableEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Siglas { get; set; } = string.Empty;
    public string LogoPath { get; set; } = string.Empty;
}
