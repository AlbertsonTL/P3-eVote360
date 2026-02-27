namespace eVote360.Application.ViewModels.Position;

public class PositionListVM
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
