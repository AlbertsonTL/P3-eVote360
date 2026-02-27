using eVote360.Application.Results;
using eVote360.Domain.Entities.Election;

namespace eVote360.Application.Abstractions.Services;

public interface IElectionService
{
    Task<Result<Election>> GetActiveElectionAsync();
    Task<Result<IEnumerable<Election>>> GetAllAsync();
    Task<Result<Election>> GetByIdAsync(int id);
    Task<Result<Election>> CreateElectionAsync(string nombre, DateTime fecha);
    Task<Result> FinalizeElectionAsync(int electionId);
    Task<Result<List<string>>> ValidateCanCreateElectionAsync();
}
