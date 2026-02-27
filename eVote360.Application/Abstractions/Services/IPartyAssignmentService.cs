using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;

namespace eVote360.Application.Abstractions.Services;

public interface IPartyAssignmentService
{
    Task<Result<IEnumerable<PartyAssignmentResponseDto>>> GetAllAsync();
    Task<Result<PartyAssignmentResponseDto>> CreateAsync(PartyAssignmentCreateDto dto);
    Task<Result> DeleteAsync(int id);
}
