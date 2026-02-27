using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;

namespace eVote360.Application.Abstractions.Services;

public interface ICandidateService
{
    Task<Result<IEnumerable<CandidateResponseDto>>> GetAllAsync();
    Task<Result<IEnumerable<CandidateResponseDto>>> GetByPartyIdAsync(int partyId);
    Task<Result<CandidateResponseDto>> GetByIdAsync(int id);
    Task<Result<CandidateResponseDto>> CreateAsync(CandidateCreateRequestDto dto, string fotoPath);
    Task<Result<CandidateResponseDto>> UpdateAsync(CandidateUpdateRequestDto dto, string? fotoPath);
    Task<Result> ToggleActiveAsync(int id);
    Task<Result> AssignPositionAsync(int candidateId, int positionId, int partyId);
}
