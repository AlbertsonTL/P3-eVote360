using eVote360.Application.Results;
using eVote360.Application.ViewModels.Admin;

namespace eVote360.Application.Abstractions.Services;

public interface IAdminDashboardService
{
    Task<Result<AdminResumenVm>> GetResumenElectoralAsync(int year);
    Task<List<int>> GetAvailableYearsAsync();
}
