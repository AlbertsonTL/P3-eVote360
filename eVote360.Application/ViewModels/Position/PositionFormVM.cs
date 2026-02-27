using System.ComponentModel.DataAnnotations;

namespace eVote360.Application.ViewModels.Position;

public class PositionFormVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria")]
    public string Descripcion { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}
