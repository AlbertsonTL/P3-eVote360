using eVote360.Application.Abstractions.Services;
using eVote360.Domain.Entities;

namespace eVote360.Application.Services;

public class UserSessionService : IUserSession
{
    private Usuario? _currentUser;

    public Usuario? GetCurrentUser() => _currentUser;

    public void SetUser(Usuario usuario) => _currentUser = usuario;

    public void Clear() => _currentUser = null;

    public bool IsAuthenticated() => _currentUser != null;

    public bool IsInRole(string role) => _currentUser?.Rol == role;
}
