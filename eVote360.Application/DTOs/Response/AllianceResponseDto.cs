namespace eVote360.Application.DTOs.Response;

public class AllianceResponseDto
{
    public int Id { get; set; }
    public int PartyOrigenId { get; set; }
    public string PartyOrigenNombre { get; set; } = string.Empty;
    public string PartyOrigenSiglas { get; set; } = string.Empty;
    public int PartyDestinoId { get; set; }
    public string PartyDestinoNombre { get; set; } = string.Empty;
    public string PartyDestinoSiglas { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaRespuesta { get; set; }
}
