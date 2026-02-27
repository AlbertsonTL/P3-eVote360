using Microsoft.AspNetCore.Mvc;
using eVote360.Application.Abstractions.Services;

namespace eVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CandidateController : Controller
{
    private readonly ICandidateService _candidateService;
    private readonly IElectionService _electionService;

    public CandidateController(ICandidateService candidateService, IElectionService electionService)
    {
        _candidateService = candidateService;
        _electionService = electionService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _candidateService.GetAllAsync();
        var activeElection = await _electionService.GetActiveElectionAsync();
        ViewBag.HasActiveElection = activeElection.IsSuccess;
        return View(result.Data?.ToList() ?? new());
    }
}
