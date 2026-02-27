namespace eVote360.Application.DTOs.Request;

public class PositionUpdateRequest
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
