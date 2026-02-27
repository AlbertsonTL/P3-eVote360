using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace eVote360.Web.Models;

public class ElectorViewModels
{
    public class MensajeVM
    {
        public string Mensaje { get; set; } = string.Empty;
        public string TipoMensaje { get; set; } = "info";
    }

    public class ValidarIdentidadVM
    {
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe subir una foto de su cédula")]
        public IFormFile? FotoCedula { get; set; }
    }

    public class OpcionesPuestosVM
    {
        public string NumeroDocumento { get; set; } = string.Empty;
        public List<PuestoDisponibleVM> Puestos { get; set; } = new();
    }

    public class PuestoDisponibleVM
    {
        public int PuestoId { get; set; }
        public string NombrePuesto { get; set; } = string.Empty;
        public int CantidadCandidatos { get; set; }
        public int CantidadPartidos { get; set; }
        public bool YaVoto { get; set; }
    }

    public class BoletaVM
    {
        public int PuestoId { get; set; }
        public string NombrePuesto { get; set; } = string.Empty;
        public string NumeroDocumento { get; set; } = string.Empty;
        public List<CandidatoBoletaVM> Candidatos { get; set; } = new();
        public int? CandidatoSeleccionadoId { get; set; }
    }

    public class CandidatoBoletaVM
    {
        public int CandidatoId { get; set; }
        public string NombreCandidato { get; set; } = string.Empty;
        public string ApellidoCandidato { get; set; } = string.Empty;
        public string FotoCandidato { get; set; } = string.Empty;
        public string LogoPartido { get; set; } = string.Empty;
        public string SiglasPartido { get; set; } = string.Empty;
        public string NombrePartido { get; set; } = string.Empty;
    }
}

// Compatibility classes
public class IdentityValidationVM
{
    public string NumeroDocumento { get; set; } = string.Empty;
}

public class VotingPositionVM
{
    public int PositionId { get; set; }
    public string PositionName { get; set; } = string.Empty;
    public int PartiesCount { get; set; }
    public int CandidatesCount { get; set; }
}

public class CandidateVoteVM
{
    public int? CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string CandidatePhoto { get; set; } = string.Empty;
    public int PartyId { get; set; }
    public string PartyLogo { get; set; } = string.Empty;
    public string PartySiglas { get; set; } = string.Empty;
}
