namespace eVote360.Application.DTOs.Response;

public class VoteResponseDto
{
    public int Id { get; set; }
    public int CitizenId { get; set; }
    public int ElectionId { get; set; }
    public DateTime FechaVoto { get; set; }
}
