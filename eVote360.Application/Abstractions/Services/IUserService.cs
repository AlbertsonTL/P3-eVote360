using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;

namespace eVote360.Application.Abstractions.Services;

public interface IUserService
{
    Task<Result<IEnumerable<UserResponseDto>>> GetAllAsync();
    Task<Result<UserResponseDto>> GetByIdAsync(int id);
    Task<Result<UserResponseDto>> CreateAsync(UserCreateRequestDto dto);
    Task<Result<UserResponseDto>> UpdateAsync(UserUpdateRequestDto dto);
    Task<Result> ToggleActiveAsync(int id);
}
