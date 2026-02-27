namespace eVote360.Application.ViewModels.Alliances;

public class AllianceListVM
{
    public int Id { get; set; }
    public string PartyOrigenNombre { get; set; } = string.Empty;
    public string PartyOrigenSiglas { get; set; } = string.Empty;
    public string PartyDestinoNombre { get; set; } = string.Empty;
    public string PartyDestinoSiglas { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaRespuesta { get; set; }
    public bool EsRecibida { get; set; } // vista desde la perspectiva del partido destino
    public int PartyOrigenId { get; set; }
    public int PartyDestinoId { get; set; }
}
