using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;

namespace eVote360.Application.Abstractions.Services;

public interface IAllianceService
{
    Task<Result<IEnumerable<AllianceResponseDto>>> GetByPartyIdAsync(int partyId);
    Task<Result<AllianceResponseDto>> CreateRequestAsync(int partyOrigenId, int partyDestinoId);
    Task<Result> AcceptAsync(int allianceId, int partyDestinoId);
    Task<Result> RejectAsync(int allianceId, int partyDestinoId);
    Task<bool> HasActiveAllianceAsync(int partyAId, int partyBId);
    Task<IEnumerable<int>> GetAlliedPartyIdsAsync(int partyId);
}
