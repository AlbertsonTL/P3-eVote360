using eVote360.Domain.Base;

namespace eVote360.Domain.Entities.Vote;

public class VoteItem : BaseEntity
{
    public int VoteId { get; set; }
    public Vote Vote { get; set; } = null!;
    public int PositionId { get; set; }
    public Position.Position Position { get; set; } = null!;
    public int? CandidateId { get; set; }  // Null si vota por "Ninguno"
    public Candidate.Candidate? Candidate { get; set; }
    public int? PartyId { get; set; }
    public Party.Party? Party { get; set; }
}
