namespace eVote360.Application.DTOs.Response;

public class ElectionResponseDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaRealizacion { get; set; }
    public string Estado { get; set; } = string.Empty;
    public bool EsActiva { get; set; }
    public DateTime CreatedAt { get; set; }
}
