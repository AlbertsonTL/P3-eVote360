using eVote360.Domain.Entities;

namespace eVote360.Application.Abstractions.Services;

public interface IUserSession
{
    Usuario? GetCurrentUser();
    void SetUser(Usuario usuario);
    void Clear();
    bool IsAuthenticated();
    bool IsInRole(string role);
}
