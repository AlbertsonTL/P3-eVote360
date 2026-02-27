namespace eVote360.Application.DTOs.Request;

public class CitizenUpdateRequestDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
