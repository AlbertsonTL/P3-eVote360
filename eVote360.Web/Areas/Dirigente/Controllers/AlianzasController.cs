using Microsoft.AspNetCore.Mvc;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.ViewModels.Alliances;

namespace eVote360.Web.Areas.Dirigente.Controllers;

[Area("Dirigente")]
public class AlianzasController : Controller
{
    private readonly IAllianceService _allianceService;
    private readonly IPartyService _partyService;
    private readonly IElectionService _electionService;

    public AlianzasController(
        IAllianceService allianceService,
        IPartyService partyService,
        IElectionService electionService)
    {
        _allianceService = allianceService;
        _partyService = partyService;
        _electionService = electionService;
    }

    private int? GetPartyId() => HttpContext.Session.GetInt32("PartyId");

    public async Task<IActionResult> Index()
    {
        var partyId = GetPartyId();
        if (!partyId.HasValue) return RedirectToAction("Index", "Login", new { area = "" });

        var result = await _allianceService.GetByPartyIdAsync(partyId.Value);
        var alliances = result.Data?.ToList() ?? new();

        var viewModels = alliances.Select(a => new AllianceListVM
        {
            Id = a.Id,
            PartyOrigenId = a.PartyOrigenId,
            PartyOrigenNombre = a.PartyOrigenNombre,
            PartyOrigenSiglas = a.PartyOrigenSiglas,
            PartyDestinoId = a.PartyDestinoId,
            PartyDestinoNombre = a.PartyDestinoNombre,
            PartyDestinoSiglas = a.PartyDestinoSiglas,
            Estado = a.Estado,
            FechaSolicitud = a.FechaSolicitud,
            FechaRespuesta = a.FechaRespuesta,
            EsRecibida = a.PartyDestinoId == partyId.Value
        }).ToList();

        var activeElection = await _electionService.GetActiveElectionAsync();
        ViewBag.HasActiveElection = activeElection.IsSuccess;
        ViewBag.PartyId = partyId;

        return View(viewModels);
    }

    [HttpGet]
    public async Task<IActionResult> Solicitar()
    {
        var partyId = GetPartyId();
        if (!partyId.HasValue) return RedirectToAction("Index", "Login", new { area = "" });

        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden crear solicitudes de alianza durante una elección activa";
            return RedirectToAction("Index");
        }

        var partiesResult = await _partyService.GetAllAsync();
        // Exclude own party and show only active parties
        ViewBag.Partidos = partiesResult.Data?
            .Where(p => p.Id != partyId.Value && p.IsActive)
            .ToList() ?? new();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Solicitar(int partyDestinoId)
    {
        var partyId = GetPartyId();
        if (!partyId.HasValue) return RedirectToAction("Index", "Login", new { area = "" });

        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden crear solicitudes de alianza durante una elección activa";
            return RedirectToAction("Index");
        }

        var result = await _allianceService.CreateRequestAsync(partyId.Value, partyDestinoId);

        if (!result.IsSuccess)
            TempData["Error"] = result.Message;
        else
            TempData["Success"] = "Solicitud de alianza enviada exitosamente";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Aceptar(int id)
    {
        var partyId = GetPartyId();
        if (!partyId.HasValue) return RedirectToAction("Index", "Login", new { area = "" });

        var result = await _allianceService.AcceptAsync(id, partyId.Value);

        if (!result.IsSuccess)
            TempData["Error"] = result.Message;
        else
            TempData["Success"] = "Alianza aceptada exitosamente";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Rechazar(int id)
    {
        var partyId = GetPartyId();
        if (!partyId.HasValue) return RedirectToAction("Index", "Login", new { area = "" });

        var result = await _allianceService.RejectAsync(id, partyId.Value);

        if (!result.IsSuccess)
            TempData["Error"] = result.Message;
        else
            TempData["Success"] = "Solicitud de alianza rechazada";

        return RedirectToAction("Index");
    }
}
