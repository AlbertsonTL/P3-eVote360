namespace eVote360.Application.DTOs.Request;

public class PartyUpdateRequest
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Siglas { get; set; } = string.Empty;
}
