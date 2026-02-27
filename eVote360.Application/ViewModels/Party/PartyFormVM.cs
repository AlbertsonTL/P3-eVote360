using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eVote360.Application.ViewModels.Party;

public class PartyFormVM
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "Las siglas son obligatorias")]
    public string Siglas { get; set; } = string.Empty;

    public IFormFile? Logo { get; set; }
    public string? LogoPath { get; set; }
    public bool IsActive { get; set; }
}
