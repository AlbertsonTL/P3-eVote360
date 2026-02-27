namespace eVote360.Application.ViewModels.Admin;

public class AdminResumenVm
{
    public List<ElectionSummary> Elections { get; set; } = new();
}

public class ElectionSummary
{
    public string NombreEleccion { get; set; } = string.Empty;
    public int CantidadPartidos { get; set; }
    public int CantidadCandidatos { get; set; }
    public int TotalVotos { get; set; }
}
