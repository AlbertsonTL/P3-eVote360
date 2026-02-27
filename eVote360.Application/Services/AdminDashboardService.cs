using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.Results;
using eVote360.Application.ViewModels.Admin;

namespace eVote360.Application.Services;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IElectionRepository _electionRepository;
    private readonly IVoteRepository _voteRepository;

    public AdminDashboardService(IElectionRepository electionRepository, IVoteRepository voteRepository)
    {
        _electionRepository = electionRepository;
        _voteRepository = voteRepository;
    }

    public async Task<Result<AdminResumenVm>> GetResumenElectoralAsync(int year)
    {
        var elections = await _electionRepository.GetElectionsByYearAsync(year);
        var resumen = new AdminResumenVm();

        // Simplified implementation
        return Result<AdminResumenVm>.Success(resumen);
    }

    public async Task<List<int>> GetAvailableYearsAsync()
    {
        var elections = await _electionRepository.GetAllAsync();
        return elections.Select(e => e.FechaRealizacion.Year).Distinct().OrderByDescending(y => y).ToList();
    }
}
