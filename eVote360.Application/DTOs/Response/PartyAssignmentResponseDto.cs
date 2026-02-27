namespace eVote360.Application.DTOs.Response;

public class PartyAssignmentResponseDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string UsuarioNombre { get; set; } = string.Empty;
    public string UserNombre { get; set; } = string.Empty; // Alias
    public int PartyId { get; set; }
    public string PartySiglas { get; set; } = string.Empty;
    public string PartidoSiglas { get; set; } = string.Empty; // Alias
    public string PartidoNombre { get; set; } = string.Empty;
}
