using eVote360.Application.DTOs.Request;
using eVote360.Application.Results;

namespace eVote360.Application.Abstractions.Services;

public interface IVotacionService
{
    Task<Result<bool>> HasVotedAsync(int citizenId, int electionId);
    Task<Result> CastVoteAsync(VoteReceiptRequestDto dto);
}
