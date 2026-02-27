namespace eVote360.Application.ViewModels.Elections;

public class ResultadosEleccionVM
{
    public int EleccionId { get; set; }
    public string EleccionNombre { get; set; } = string.Empty;
    public DateTime FechaRealizacion { get; set; }
    public int TotalVotos { get; set; }
    public List<ResultadoPuestoVM> Puestos { get; set; } = new();
}

public class ResultadoPuestoVM
{
    public int PuestoId { get; set; }
    public string PuestoNombre { get; set; } = string.Empty;
    public int TotalVotosPuesto { get; set; }
    public List<ResultadoCandidatoVM> Candidatos { get; set; } = new();
}

public class ResultadoCandidatoVM
{
    public string CandidatoNombre { get; set; } = string.Empty;
    public string PartidoNombre { get; set; } = string.Empty;
    public string PartidoSiglas { get; set; } = string.Empty;
    public int Votos { get; set; }
    public double Porcentaje { get; set; }
}
