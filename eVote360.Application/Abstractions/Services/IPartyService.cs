using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;

namespace eVote360.Application.Abstractions.Services;

public interface IPartyService
{
    Task<Result<IEnumerable<PartyResponse>>> GetAllAsync();
    Task<Result<PartyResponse>> GetByIdAsync(int id);
    Task<Result<PartyResponse>> CreateAsync(PartyCreateRequest dto, string logoPath);
    Task<Result<PartyResponse>> UpdateAsync(PartyUpdateRequest dto, string? logoPath);
    Task<Result> ToggleActiveAsync(int id);
}
