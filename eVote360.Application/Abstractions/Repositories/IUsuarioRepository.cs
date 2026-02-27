using eVote360.Domain.Entities;

namespace eVote360.Application.Abstractions.Repositories;

public interface IUsuarioRepository : IGenericRepository<Usuario>
{
    Task<Usuario?> GetByUsernameAsync(string username);
}
