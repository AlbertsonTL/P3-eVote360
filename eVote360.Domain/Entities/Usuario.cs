using eVote360.Domain.Base;
using eVote360.Domain.ValueObjects;

namespace eVote360.Domain.Entities;

public class Usuario : AuditableEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public EmailAddress Email { get; set; } = null!;
    public string NombreUsuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty; // "Administrador" o "Dirigente"
}
