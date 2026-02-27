using System.Collections.Concurrent;

namespace eVote360.Infrastructure.Email;

public class InMemoryEmailQueue
{
    private readonly ConcurrentQueue<EmailMessage> _queue = new();

    public void Enqueue(EmailMessage message)
    {
        _queue.Enqueue(message);
    }

    public bool TryDequeue(out EmailMessage? message)
    {
        return _queue.TryDequeue(out message);
    }

    public bool IsEmpty => _queue.IsEmpty;
}

public class EmailMessage
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
