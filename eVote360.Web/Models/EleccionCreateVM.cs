using System.ComponentModel.DataAnnotations;

namespace eVote360.Web.Models;

public class EleccionCreateVM
{
    [Required(ErrorMessage = "El nombre de la elección es obligatorio")]
    [Display(Name = "Nombre de la Elección")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de realización es obligatoria")]
    [Display(Name = "Fecha de Realización")]
    [DataType(DataType.Date)]
    public DateTime FechaRealizacion { get; set; } = DateTime.Today;
}

public class ResultadosEleccionVM
{
    public int EleccionId { get; set; }
    public string NombreEleccion { get; set; } = string.Empty;
    public DateTime FechaRealizacion { get; set; }
    public List<ResultadoPuestoVM> Puestos { get; set; } = new();
}

public class ResultadoPuestoVM
{
    public int PuestoId { get; set; }
    public string NombrePuesto { get; set; } = string.Empty;
    public int TotalVotos { get; set; }
    public List<ResultadoCandidatoVM> Candidatos { get; set; } = new();
}

public class ResultadoCandidatoVM
{
    public int? CandidatoId { get; set; }
    public string NombreCandidato { get; set; } = string.Empty;
    public string NombrePartido { get; set; } = string.Empty;
    public string SiglasPartido { get; set; } = string.Empty;
    public int Votos { get; set; }
    public decimal Porcentaje { get; set; }
}
