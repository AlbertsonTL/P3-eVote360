using System.ComponentModel.DataAnnotations;

namespace eVote360.Application.DTOs.Request;

public class CandidateCreateRequestDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = string.Empty;

    public int PartyId { get; set; }
}
