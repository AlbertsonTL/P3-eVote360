using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.ViewModels.Candidates;
using eVote360.Domain.Entities;
using eVote360.Infrastructure.Persistence;

namespace eVote360.Web.Areas.Dirigente.Controllers;

[Area("Dirigente")]
public class CandidatoPuestoController : Controller
{
    private readonly ICandidateRepository _candidateRepo;
    private readonly IPositionService _positionService;
    private readonly IPartyAssignmentRepository _assignmentRepo;
    private readonly IElectionService _electionService;
    private readonly IAllianceService _allianceService;
    private readonly AppDbContext _db;

    public CandidatoPuestoController(
        ICandidateRepository candidateRepo,
        IPositionService positionService,
        IPartyAssignmentRepository assignmentRepo,
        IElectionService electionService,
        IAllianceService allianceService,
        AppDbContext db)
    {
        _candidateRepo = candidateRepo;
        _positionService = positionService;
        _assignmentRepo = assignmentRepo;
        _electionService = electionService;
        _allianceService = allianceService;
        _db = db;
    }

    private async Task<int?> GetMyPartyIdAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return null;
        var assignment = await _assignmentRepo.GetByUsuarioIdAsync(userId.Value);
        return assignment?.PartyId;
    }

    public async Task<IActionResult> Index()
    {
        var partyId = await GetMyPartyIdAsync();
        if (partyId == null) return RedirectToAction("Index", "Login", new { area = "" });

        var candidaturas = await _db.Candidaturas
            .Include(c => c.Candidate).ThenInclude(c => c.Party)
            .Include(c => c.Position)
            .Where(c => c.PartyId == partyId.Value)
            .ToListAsync();

        var hasActive = (await _electionService.GetActiveElectionAsync()).IsSuccess;
        ViewBag.HasActiveElection = hasActive;
        return View(candidaturas);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var partyId = await GetMyPartyIdAsync();
        if (partyId == null) return RedirectToAction("Index", "Login", new { area = "" });

        if ((await _electionService.GetActiveElectionAsync()).IsSuccess)
        {
            TempData["Error"] = "No se pueden asignar candidatos mientras hay una eleccin activa.";
            return RedirectToAction("Index");
        }

        await LoadCreateViewBag(partyId.Value);
        return View(new CandidatoPuestoCreateVM { PartyId = partyId.Value });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CandidatoPuestoCreateVM model)
    {
        var partyId = await GetMyPartyIdAsync();
        if (partyId == null) return RedirectToAction("Index", "Login", new { area = "" });

        if ((await _electionService.GetActiveElectionAsync()).IsSuccess)
        {
            TempData["Error"] = "No se pueden asignar candidatos mientras hay una elección activa.";
            return RedirectToAction("Index");
        }

        model.PartyId = partyId.Value;

        // Rule: position not already taken in this party
        var positionTaken = await _db.Candidaturas
            .AnyAsync(c => c.PartyId == partyId.Value && c.PositionId == model.PositionId);
        if (positionTaken)
        {
            ModelState.AddModelError("", "Ya existe un candidato asignado a ese puesto en su partido.");
            await LoadCreateViewBag(partyId.Value);
            return View(model);
        }

        // Rule: candidate not already assigned in this party
        var candidateAssigned = await _db.Candidaturas
            .AnyAsync(c => c.PartyId == partyId.Value && c.CandidateId == model.CandidateId);
        if (candidateAssigned)
        {
            ModelState.AddModelError("", "Este candidato ya está asignado a un puesto dentro del partido.");
            await LoadCreateViewBag(partyId.Value);
            return View(model);
        }

        // Alliance rule: candidate from allied party must aspire same position as in origin party
        var candidate = await _candidateRepo.GetByIdAsync(model.CandidateId);
        if (candidate != null && candidate.PartyId != partyId.Value)
        {
            var originCandidatura = await _db.Candidaturas
                .FirstOrDefaultAsync(c => c.CandidateId == model.CandidateId && c.PartyId == candidate.PartyId);

            if (originCandidatura != null && originCandidatura.PositionId != model.PositionId)
            {
                ModelState.AddModelError("", "Este candidato en su partido de origen aspira a un puesto diferente al seleccionado. Un candidato en alianza solo puede postular al mismo puesto.");
                await LoadCreateViewBag(partyId.Value);
                return View(model);
            }
        }

        _db.Candidaturas.Add(new Candidatura
        {
            CandidateId = model.CandidateId,
            PositionId = model.PositionId,
            PartyId = partyId.Value
        });
        await _db.SaveChangesAsync();

        // Update candidate PositionId if own party
        if (candidate != null && candidate.PartyId == partyId.Value && candidate.PositionId == null)
        {
            candidate.PositionId = model.PositionId;
            await _candidateRepo.UpdateAsync(candidate);
        }

        TempData["Success"] = "Candidato asignado al puesto exitosamente.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var partyId = await GetMyPartyIdAsync();
        if (partyId == null) return RedirectToAction("Index", "Login", new { area = "" });

        if ((await _electionService.GetActiveElectionAsync()).IsSuccess)
        {
            TempData["Error"] = "No se puede eliminar asignaciones mientras hay una eleccin activa.";
            return RedirectToAction("Index");
        }

        var candidatura = await _db.Candidaturas
            .Include(c => c.Candidate)
            .Include(c => c.Position)
            .FirstOrDefaultAsync(c => c.Id == id && c.PartyId == partyId.Value);

        if (candidatura == null) return NotFound();
        ViewBag.CandidaturaNombre = $"{candidatura.Candidate?.Nombre} {candidatura.Candidate?.Apellido}";
        ViewBag.PuestoNombre = candidatura.Position?.Nombre;
        ViewBag.CandidaturaId = id;
        return View();
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirm(int id)
    {
        var partyId = await GetMyPartyIdAsync();
        if (partyId == null) return RedirectToAction("Index", "Login", new { area = "" });

        var candidatura = await _db.Candidaturas
            .FirstOrDefaultAsync(c => c.Id == id && c.PartyId == partyId.Value);
        if (candidatura != null)
        {
            var candidate = await _candidateRepo.GetByIdAsync(candidatura.CandidateId);
            if (candidate != null && candidate.PartyId == partyId.Value)
            {
                candidate.PositionId = null;
                await _candidateRepo.UpdateAsync(candidate);
            }
            _db.Candidaturas.Remove(candidatura);
            await _db.SaveChangesAsync();
        }
        TempData["Success"] = "Asignacin eliminada exitosamente.";
        return RedirectToAction("Index");
    }

    private async Task LoadCreateViewBag(int partyId)
    {
        var assignedCandidateIds = await _db.Candidaturas
            .Where(c => c.PartyId == partyId)
            .Select(c => c.CandidateId)
            .ToListAsync();

        var ownCandidates = (await _candidateRepo.GetByPartyIdAsync(partyId))
            .Where(c => c.IsActive && !assignedCandidateIds.Contains(c.Id))
            .ToList();

        // Allied candidates
        var alliedIds = await _allianceService.GetAlliedPartyIdsAsync(partyId);
        var alliedCandidates = new List<eVote360.Domain.Entities.Candidate.Candidate>();
        foreach (var allyId in alliedIds)
        {
            var allied = (await _candidateRepo.GetByPartyIdAsync(allyId))
                .Where(c => c.IsActive && !assignedCandidateIds.Contains(c.Id))
                .ToList();
            alliedCandidates.AddRange(allied);
        }

        ViewBag.Candidatos = ownCandidates.Concat(alliedCandidates).ToList();

        var occupiedPositionIds = await _db.Candidaturas
            .Where(c => c.PartyId == partyId)
            .Select(c => c.PositionId)
            .ToListAsync();

        var positionsResult = await _positionService.GetAllAsync();
        ViewBag.Puestos = positionsResult.Data?
            .Where(p => p.IsActive && !occupiedPositionIds.Contains(p.Id))
            .ToList() ?? new List<eVote360.Application.DTOs.Response.PositionResponse>();
    }
}
