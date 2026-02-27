using Microsoft.Extensions.DependencyInjection;
using eVote360.Application.Services;
using eVote360.Application.Abstractions.Services;

namespace eVote360.Application.Common;

public static class ServicesRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ServicesRegistration).Assembly);

        services.AddScoped<ICitizenService, CitizenService>();
        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<IPartyService, PartyService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICandidateService, CandidateService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IElectionService, ElectionService>();
        services.AddScoped<IVotacionService, VotacionService>();
        services.AddScoped<IPartyAssignmentService, PartyAssignmentService>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();
        services.AddScoped<IAllianceService, AllianceService>();
        // IOcrService se registra en Infrastructure (TesseractOcrService)

        return services;
    }
}
