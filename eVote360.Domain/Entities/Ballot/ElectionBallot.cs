using eVote360.Domain.Base;

namespace eVote360.Domain.Entities.Ballot;

public class ElectionBallot : BaseEntity
{
    public int ElectionId { get; set; }
    public Election.Election Election { get; set; } = null!;
    public int PositionId { get; set; }
    public Position.Position Position { get; set; } = null!;
}
