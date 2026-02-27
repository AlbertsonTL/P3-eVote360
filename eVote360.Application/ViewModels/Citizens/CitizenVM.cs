using System.ComponentModel.DataAnnotations;

namespace eVote360.Application.ViewModels.Citizens;

public class CitizenVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de documento es obligatorio")]
    [StringLength(20)]
    public string NumeroDocumento { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
