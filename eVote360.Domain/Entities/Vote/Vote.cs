using eVote360.Domain.Base;

namespace eVote360.Domain.Entities.Vote;

public class Vote : BaseEntity
{
    public int CitizenId { get; set; }
    public Citizen.Citizen Citizen { get; set; } = null!;
    public int ElectionId { get; set; }
    public Election.Election Election { get; set; } = null!;
    public DateTime FechaVoto { get; set; } = DateTime.UtcNow;
    public ICollection<VoteItem> VoteItems { get; set; } = new List<VoteItem>();
}
