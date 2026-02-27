using Microsoft.AspNetCore.Mvc;
using eVote360.Application.Abstractions.Services;

namespace eVote360.Web.ViewComponents;

public class ActiveElectionBannerViewComponent : ViewComponent
{
    private readonly IElectionService _electionService;

    public ActiveElectionBannerViewComponent(IElectionService electionService)
    {
        _electionService = electionService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var result = await _electionService.GetActiveElectionAsync();
        return View(result.Data);
    }
}
