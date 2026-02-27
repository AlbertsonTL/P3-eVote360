using System.ComponentModel.DataAnnotations;

namespace eVote360.Application.ViewModels.Elections;

public class ElectionFormVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la elección es obligatorio")]
    [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
    [Display(Name = "Nombre de la Elección")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de realización es obligatoria")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de Realización")]
    public DateTime FechaRealizacion { get; set; } = DateTime.Today.AddDays(7);
}
