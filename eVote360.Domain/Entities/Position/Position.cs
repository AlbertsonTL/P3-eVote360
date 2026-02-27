using eVote360.Domain.Base;

namespace eVote360.Domain.Entities.Position;

public class Position : AuditableEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
