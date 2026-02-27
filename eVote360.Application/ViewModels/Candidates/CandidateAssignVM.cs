using System.ComponentModel.DataAnnotations;

namespace eVote360.Application.ViewModels.Candidates;

public class CandidateAssignVM
{
    [Required(ErrorMessage = "Debe seleccionar un candidato")]
    [Display(Name = "Candidato")]
    public int CandidateId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un puesto")]
    [Display(Name = "Puesto Electivo")]
    public int PositionId { get; set; }
}
