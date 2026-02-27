using eVote360.Domain.Base;

namespace eVote360.Domain.Entities;

public class Candidatura : BaseEntity
{
    public int CandidateId { get; set; }
    public Candidate.Candidate Candidate { get; set; } = null!;
    public int PositionId { get; set; }
    public Position.Position Position { get; set; } = null!;
    public int PartyId { get; set; }
    public Party.Party Party { get; set; } = null!;
}
