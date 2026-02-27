using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;

namespace eVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class AsignacionesController : Controller
{
    private readonly IPartyAssignmentService _assignmentService;
    private readonly IUserService _userService;
    private readonly IPartyService _partyService;
    private readonly IElectionService _electionService;
    private readonly IMapper _mapper;

    public AsignacionesController(
        IPartyAssignmentService assignmentService,
        IUserService userService,
        IPartyService partyService,
        IElectionService electionService,
        IMapper mapper)
    {
        _assignmentService = assignmentService;
        _userService = userService;
        _partyService = partyService;
        _electionService = electionService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Dirigentes()
    {
        var result = await _assignmentService.GetAllAsync();
        var hasActive = (await _electionService.GetActiveElectionAsync()).IsSuccess;
        ViewBag.HasActiveElection = hasActive;
        return View(result.Data?.ToList() ?? new List<PartyAssignmentResponseDto>());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var hasActive = (await _electionService.GetActiveElectionAsync()).IsSuccess;
        if (hasActive)
        {
            TempData["Error"] = "No se pueden crear asignaciones mientras hay una elección activa.";
            return RedirectToAction("Dirigentes");
        }
        await LoadViewBag();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PartyAssignmentCreateDto dto)
    {
        var hasActive = (await _electionService.GetActiveElectionAsync()).IsSuccess;
        if (hasActive)
        {
            TempData["Error"] = "No se pueden crear asignaciones mientras hay una elección activa.";
            return RedirectToAction("Dirigentes");
        }

        var result = await _assignmentService.CreateAsync(dto);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            await LoadViewBag();
            return View(dto);
        }

        TempData["Success"] = "Asignación creada exitosamente.";
        return RedirectToAction("Dirigentes");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var hasActive = (await _electionService.GetActiveElectionAsync()).IsSuccess;
        if (hasActive)
        {
            TempData["Error"] = "No se pueden eliminar asignaciones mientras hay una elección activa.";
            return RedirectToAction("Dirigentes");
        }

        await _assignmentService.DeleteAsync(id);
        TempData["Success"] = "Asignación eliminada exitosamente.";
        return RedirectToAction("Dirigentes");
    }

    private async Task LoadViewBag()
    {
        var users = await _userService.GetAllAsync();
        ViewBag.Dirigentes = users.Data?
            .Where(u => u.Rol == "Dirigente" && u.IsActive)
            .ToList() ?? new List<UserResponseDto>();
        var parties = await _partyService.GetAllAsync();
        ViewBag.Partidos = parties.Data?
            .Where(p => p.IsActive)
            .ToList() ?? new List<PartyResponse>();
    }
}
