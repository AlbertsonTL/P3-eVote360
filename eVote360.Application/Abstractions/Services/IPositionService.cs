using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;

namespace eVote360.Application.Abstractions.Services;

public interface IPositionService
{
    Task<Result<IEnumerable<PositionResponse>>> GetAllAsync();
    Task<Result<PositionResponse>> GetByIdAsync(int id);
    Task<Result<PositionResponse>> CreateAsync(PositionCreateRequest dto);
    Task<Result<PositionResponse>> UpdateAsync(PositionUpdateRequest dto);
    Task<Result> ToggleActiveAsync(int id);
}
