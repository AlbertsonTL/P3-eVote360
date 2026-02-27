using System.ComponentModel.DataAnnotations;

namespace eVote360.Application.ViewModels.Users;

public class UserVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    public string NombreUsuario { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string? ConfirmPassword { get; set; }

    [Required(ErrorMessage = "El rol es obligatorio")]
    public string Rol { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
