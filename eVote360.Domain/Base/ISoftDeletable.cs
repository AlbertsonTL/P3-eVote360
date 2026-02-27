namespace eVote360.Domain.Base;

public interface ISoftDeletable
{
    bool IsActive { get; set; }
}
