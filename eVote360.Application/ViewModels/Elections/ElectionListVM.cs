namespace eVote360.Application.ViewModels.Elections;

public class ElectionListVM
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaRealizacion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool EsActiva { get; set; }
    public int TotalVotos { get; set; }
    public DateTime CreatedAt { get; set; }
}
