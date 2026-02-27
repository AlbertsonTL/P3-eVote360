using eVote360.Domain.Entities.Position;

namespace eVote360.Application.Abstractions.Repositories;

public interface IPositionRepository : IGenericRepository<Position>
{
    Task<IEnumerable<Position>> GetActivePositionsAsync();
}
