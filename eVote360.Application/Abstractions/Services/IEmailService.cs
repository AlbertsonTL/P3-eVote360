namespace eVote360.Application.Abstractions.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendVotingConfirmationAsync(string to, string nombreCiudadano);
    Task SendVoteReceiptAsync(string to, string nombreCiudadano, string electionName, List<VoteReceiptItem> items);
}

public class VoteReceiptItem
{
    public string PuestoNombre { get; set; } = string.Empty;
    public string CandidatoNombre { get; set; } = string.Empty;
    public string PartidoNombre { get; set; } = string.Empty;
}
