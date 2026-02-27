using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using eVote360.Application.Abstractions.Emails;

namespace eVote360.Infrastructure.Email;

public class EmailBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly InMemoryEmailQueue _emailQueue;

    public EmailBackgroundService(IServiceProvider serviceProvider, InMemoryEmailQueue emailQueue)
    {
        _serviceProvider = serviceProvider;
        _emailQueue = emailQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!_emailQueue.IsEmpty)
            {
                using var scope = _serviceProvider.CreateScope();
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                while (_emailQueue.TryDequeue(out var emailMessage))
                {
                    if (emailMessage != null)
                    {
                        await emailSender.SendEmailAsync(
                            emailMessage.To,
                            emailMessage.Subject,
                            emailMessage.Body);
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
