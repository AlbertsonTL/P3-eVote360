namespace eVote360.Application.ViewModels.Party;

public class PartyListVM
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Siglas { get; set; } = string.Empty;
    public string LogoPath { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
