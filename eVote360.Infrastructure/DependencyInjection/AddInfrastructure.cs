using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using eVote360.Application.Abstractions.Emails;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.Common.Security;
using eVote360.Application.Common.Orc;
using eVote360.Infrastructure.Email;
using eVote360.Infrastructure.OCR;
using eVote360.Infrastructure.Persistence;
using eVote360.Infrastructure.Repositories;
using eVote360.Infrastructure.Security;
using eVote360.Shared.Emails;

namespace eVote360.Infrastructure.DependencyInjection;

public static class AddInfrastructure
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<ICitizenRepository, CitizenRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IPartyRepository, PartyRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ICandidateRepository, CandidateRepository>();
        services.AddScoped<IElectionRepository, ElectionRepository>();
        services.AddScoped<IVoteRepository, VoteRepository>();
        services.AddScoped<IPartyAssignmentRepository, PartyAssignmentRepository>();
        services.AddScoped<IAllianceRepository, AllianceRepository>();

        // Security
        services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();

        // OCR
        services.AddScoped<IOcrService, TesseractOcrService>();

        // Email - Configuración correcta de DI
        var emailSection = configuration.GetSection("EmailSenderOptions");
        services.Configure<EmailSenderOptions>(opts =>
        {
            opts.SmtpServer   = emailSection["SmtpServer"]   ?? "";
            opts.SmtpPort     = int.TryParse(emailSection["SmtpPort"],   out var p) ? p : 587;
            opts.SenderEmail  = emailSection["SenderEmail"]  ?? "";
            opts.SenderPassword = emailSection["SenderPassword"] ?? "";
            opts.EnableSsl    = bool.TryParse(emailSection["EnableSsl"], out var s) && s;
        });
        services.AddSingleton<InMemoryEmailQueue>();
        services.AddScoped<IEmailSender, SmtpEmailService>();
        services.AddScoped<IEmailService, EmailServiceAdapter>();
        services.AddHostedService<EmailBackgroundService>();

        return services;
    }
}
