using eVote360.Application.Results;
using eVote360.Domain.Entities;

namespace eVote360.Application.Abstractions.Services;

public interface IAuthService
{
    Task<Result<Usuario>> AuthenticateAsync(string username, string password);
}
