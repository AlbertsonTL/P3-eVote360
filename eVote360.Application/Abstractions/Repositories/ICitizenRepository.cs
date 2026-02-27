using eVote360.Domain.Entities.Citizen;

namespace eVote360.Application.Abstractions.Repositories;

public interface ICitizenRepository : IGenericRepository<Citizen>
{
    Task<Citizen?> GetByNationalIdAsync(string nationalId);
    Task<bool> ExistsDifferentCitizenWithNationalIdAsync(int citizenId, string nationalId);
}
