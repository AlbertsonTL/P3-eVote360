namespace eVote360.Application.DTOs.Request;

public class UserUpdateRequestDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string Rol { get; set; } = string.Empty;
}
