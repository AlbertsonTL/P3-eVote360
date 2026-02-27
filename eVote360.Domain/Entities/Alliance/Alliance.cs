using eVote360.Domain.Base;
using eVote360.Domain.Enums;

namespace eVote360.Domain.Entities.Alliance;

public class Alliance : BaseEntity
{
    public int PartyOrigenId { get; set; }
    public Party.Party PartyOrigen { get; set; } = null!;
    public int PartyDestinoId { get; set; }
    public Party.Party PartyDestino { get; set; } = null!;
    public AllianceStatus Estado { get; set; } = AllianceStatus.EnEsperaDeRespuesta;
    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
    public DateTime? FechaRespuesta { get; set; }
}
