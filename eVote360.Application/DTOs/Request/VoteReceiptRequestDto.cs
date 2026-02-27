namespace eVote360.Application.DTOs.Request;

public class VoteReceiptRequestDto
{
    public int CitizenId { get; set; }
    public int ElectionId { get; set; }
    public List<VoteItemDto> Items { get; set; } = new();
}

public class VoteItemDto
{
    public int PositionId { get; set; }
    public int? CandidateId { get; set; }
    public int? PartyId { get; set; }
}
