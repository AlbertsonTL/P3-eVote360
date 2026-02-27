using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.Common.Security;
using eVote360.Application.Results;
using eVote360.Domain.Entities;

namespace eVote360.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUsuarioRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<Usuario>> AuthenticateAsync(string username, string password)
    {
        var user = await _repository.GetByUsernameAsync(username);
        if (user == null)
            return Result<Usuario>.Failure("Credenciales inválidas");

        if (!user.IsActive)
            return Result<Usuario>.Failure("El usuario está inactivo");

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            return Result<Usuario>.Failure("Credenciales inválidas");

        return Result<Usuario>.Success(user);
    }
}
