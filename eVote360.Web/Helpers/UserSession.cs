using eVote360.Domain.Entities;

namespace eVote360.Web.Helpers;

public class UserSession
{
    public int UserId { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int? PartyId { get; set; }
}
