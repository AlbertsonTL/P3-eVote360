using eVote360.Domain.Base;
using eVote360.Domain.Enums;

namespace eVote360.Domain.Entities.Election;

public class Election : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaRealizacion { get; set; }
    public ElectionState Estado { get; set; } = ElectionState.EnProceso;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
