using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;

namespace eVote360.Application.Abstractions.Services;

public interface ICitizenService
{
    Task<Result<IEnumerable<CitizenResponseDto>>> GetAllAsync();
    Task<Result<CitizenResponseDto>> GetByIdAsync(int id);
    Task<Result<CitizenResponseDto>> CreateAsync(CitizenCreateRequestDto dto);
    Task<Result<CitizenResponseDto>> UpdateAsync(CitizenUpdateRequestDto dto);
    Task<Result> ToggleActiveAsync(int id);
}
