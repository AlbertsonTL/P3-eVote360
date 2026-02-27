using eVote360.Domain.Base;

namespace eVote360.Domain.Entities.Ballot;

public class BallotOption : BaseEntity
{
    public int ElectionBallotId { get; set; }
    public ElectionBallot ElectionBallot { get; set; } = null!;
    public int CandidateId { get; set; }
    public Candidate.Candidate Candidate { get; set; } = null!;
    public int PartyId { get; set; }
    public Party.Party Party { get; set; } = null!;
}
