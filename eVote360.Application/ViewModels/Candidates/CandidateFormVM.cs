using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eVote360.Application.ViewModels.Candidates;

public class CandidateFormVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    public int PartyId { get; set; }
    public string? PartyNombre { get; set; }

    public IFormFile? Foto { get; set; }
    public string? FotoPath { get; set; }
    public bool IsActive { get; set; }
}
