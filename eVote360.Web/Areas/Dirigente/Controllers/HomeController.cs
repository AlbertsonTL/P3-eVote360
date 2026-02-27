using Microsoft.AspNetCore.Mvc;
using eVote360.Application.Abstractions.Services;
using eVote360.Web.Helpers;

namespace eVote360.Web.Areas.Dirigente.Controllers;

[Area("Dirigente")]
public class HomeController : Controller
{
    private readonly ICandidateService _candidateService;
    private readonly IPositionService _positionService;
    private readonly IAllianceService _allianceService;
    private readonly IElectionService _electionService;
    private readonly IPartyService _partyService;

    public HomeController(
        ICandidateService candidateService,
        IPositionService positionService,
        IAllianceService allianceService,
        IElectionService electionService,
        IPartyService partyService)
    {
        _candidateService = candidateService;
        _positionService = positionService;
        _allianceService = allianceService;
        _electionService = electionService;
        _partyService = partyService;
    }

    public async Task<IActionResult> Index()
    {
        var partyId = HttpContext.Session.GetInt32("PartyId");
        if (!partyId.HasValue)
            return RedirectToAction("Index", "Login", new { area = "" });

        var candidatesResult = await _candidateService.GetByPartyIdAsync(partyId.Value);
        var positionsResult = await _positionService.GetAllAsync();
        var alliancesResult = await _allianceService.GetByPartyIdAsync(partyId.Value);
        var activeElectionResult = await _electionService.GetActiveElectionAsync();
        var partyResult = await _partyService.GetByIdAsync(partyId.Value);

        // Partido info
        if (partyResult.IsSuccess && partyResult.Data != null)
        {
            ViewBag.PartidoNombre = partyResult.Data.Nombre;
            ViewBag.PartidoSiglas = partyResult.Data.Siglas;
            ViewBag.PartidoLogo = partyResult.Data.LogoPath;
        }

        var alliances = alliancesResult.Data?.ToList() ?? new();

        // Solicitudes pendientes enviadas AL partido del dirigente (Estado == "Pendiente" o "EnEspera" y origen != mi partido)
        int solicitudesPendientes = alliances.Count(a =>
            a.PartyDestinoId == partyId.Value &&
            (a.Estado == "Pendiente" || a.Estado == "EnEspera" || a.Estado == "En espera de respuesta"));

        ViewBag.PartyId = partyId;
        ViewBag.TotalCandidatos = candidatesResult.Data?.Count() ?? 0;
        ViewBag.CandidatosActivos = candidatesResult.Data?.Count(c => c.IsActive) ?? 0;
        ViewBag.CandidatosInactivos = candidatesResult.Data?.Count(c => !c.IsActive) ?? 0;
        ViewBag.TotalPuestos = positionsResult.Data?.Count() ?? 0;
        ViewBag.PuestosAsignados = candidatesResult.Data?.Count(c => c.PositionId.HasValue) ?? 0;
        ViewBag.AlianzasActivas = alliances.Count(a => a.Estado == "Aceptada");
        ViewBag.SolicitudesPendientes = solicitudesPendientes;
        ViewBag.HasActiveElection = activeElectionResult.IsSuccess;

        return View();
    }
}
