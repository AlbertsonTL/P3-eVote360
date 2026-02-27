namespace eVote360.Application.DTOs.Response;

public class CandidateResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string FotoPath { get; set; } = string.Empty;
    public int PartyId { get; set; }
    public string PartyNombre { get; set; } = string.Empty;
    public int? PositionId { get; set; }
    public string? PositionNombre { get; set; }
    public bool IsActive { get; set; }
}
