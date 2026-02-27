namespace eVote360.Shared.Emails;

public class EmailSenderOptions
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderPassword { get; set; } = string.Empty;
    public bool EnableSsl { get; set; } = true;
}
