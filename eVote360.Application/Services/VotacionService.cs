using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.DTOs.Request;
using eVote360.Application.Results;
using eVote360.Domain.Entities.Vote;

namespace eVote360.Application.Services;

public class VotacionService : IVotacionService
{
    private readonly IVoteRepository _repository;

    public VotacionService(IVoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> HasVotedAsync(int citizenId, int electionId)
    {
        var hasVoted = await _repository.HasVotedInElectionAsync(citizenId, electionId);
        return Result<bool>.Success(hasVoted);
    }

    public async Task<Result> CastVoteAsync(VoteReceiptRequestDto dto)
    {
        // Use targeted query instead of loading all votes
        var citizenVote = await _repository.GetVoteByCitizenAndElectionAsync(dto.CitizenId, dto.ElectionId);

        if (citizenVote == null)
        {
            citizenVote = new Vote
            {
                CitizenId = dto.CitizenId,
                ElectionId = dto.ElectionId,
                FechaVoto = DateTime.UtcNow
            };

            foreach (var item in dto.Items)
            {
                citizenVote.VoteItems.Add(new VoteItem
                {
                    PositionId = item.PositionId,
                    CandidateId = item.CandidateId,
                    PartyId = item.PartyId
                });
            }

            await _repository.AddAsync(citizenVote);
        }
        else
        {
            // Add new vote items to existing vote (one per position)
            foreach (var item in dto.Items)
            {
                if (!citizenVote.VoteItems.Any(vi => vi.PositionId == item.PositionId))
                {
                    citizenVote.VoteItems.Add(new VoteItem
                    {
                        PositionId = item.PositionId,
                        CandidateId = item.CandidateId,
                        PartyId = item.PartyId
                    });
                }
            }
            await _repository.UpdateAsync(citizenVote);
        }

        return Result.Success();
    }
}
