using Microsoft.AspNetCore.Mvc;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.DTOs.Request;
using eVote360.Application.ViewModels.Candidates;

namespace eVote360.Web.Areas.Dirigente.Controllers;

[Area("Dirigente")]
public class CandidatosController : Controller
{
    private readonly ICandidateService _candidateService;
    private readonly IPositionService _positionService;
    private readonly IElectionService _electionService;
    private readonly IWebHostEnvironment _env;

    public CandidatosController(
        ICandidateService candidateService,
        IPositionService positionService,
        IElectionService electionService,
        IWebHostEnvironment env)
    {
        _candidateService = candidateService;
        _positionService = positionService;
        _electionService = electionService;
        _env = env;
    }

    private int? GetPartyId() => HttpContext.Session.GetInt32("PartyId");

    public async Task<IActionResult> Index()
    {
        var partyId = GetPartyId();
        if (!partyId.HasValue) return RedirectToAction("Index", "Login", new { area = "" });

        var result = await _candidateService.GetByPartyIdAsync(partyId.Value);
        var candidates = result.Data?.ToList() ?? new();

        var activeElection = await _electionService.GetActiveElectionAsync();
        ViewBag.HasActiveElection = activeElection.IsSuccess;
        ViewBag.PartyId = partyId;

        return View(candidates);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden crear candidatos durante una eleccin activa";
            return RedirectToAction("Index");
        }

        var partyId = GetPartyId();
        ViewBag.PartyId = partyId;
        return View(new CandidateFormVM { PartyId = partyId ?? 0 });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CandidateFormVM model)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden crear candidatos durante una elección activa";
            return RedirectToAction("Index");
        }

        if (!ModelState.IsValid) return View(model);

        var partyId = GetPartyId();
        if (!partyId.HasValue) return RedirectToAction("Index", "Login", new { area = "" });

        string fotoPath = "/images/candidates/default.png";
        if (model.Foto != null)
            fotoPath = await SaveFotoAsync(model.Foto);

        var dto = new CandidateCreateRequestDto
        {
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            PartyId = partyId.Value
        };

        var result = await _candidateService.CreateAsync(dto, fotoPath);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["Success"] = "Candidato creado exitosamente";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden editar candidatos durante una eleccin activa";
            return RedirectToAction("Index");
        }

        var result = await _candidateService.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        var c = result.Data!;
        var model = new CandidateFormVM
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Apellido = c.Apellido,
            PartyId = c.PartyId,
            FotoPath = c.FotoPath,
            IsActive = c.IsActive
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CandidateFormVM model)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden editar candidatos durante una elección activa";
            return RedirectToAction("Index");
        }

        if (!ModelState.IsValid) return View(model);

        string? fotoPath = null;
        if (model.Foto != null)
            fotoPath = await SaveFotoAsync(model.Foto);

        var dto = new CandidateUpdateRequestDto { Id = model.Id, Nombre = model.Nombre, Apellido = model.Apellido };
        var result = await _candidateService.UpdateAsync(dto, fotoPath);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["Success"] = "Candidato actualizado exitosamente";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Toggle(int id)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se puede cambiar estado de candidatos durante una eleccin activa";
            return RedirectToAction("Index");
        }

        await _candidateService.ToggleActiveAsync(id);
        TempData["Success"] = "Estado del candidato actualizado";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Asignar()
    {
        var partyId = GetPartyId();
        if (!partyId.HasValue) return RedirectToAction("Index", "Login", new { area = "" });

        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden asignar candidatos durante una eleccin activa";
            return RedirectToAction("Index");
        }

        var candidatesResult = await _candidateService.GetByPartyIdAsync(partyId.Value);
        var positionsResult = await _positionService.GetAllAsync();

        ViewBag.Candidatos = candidatesResult.Data?.Where(c => c.IsActive).ToList() ?? new();
        ViewBag.Puestos = positionsResult.Data?.Where(p => p.IsActive).ToList() ?? new();

        return View(new CandidateAssignVM());
    }

    [HttpPost]
    public async Task<IActionResult> Asignar(CandidateAssignVM model)
    {
        var partyId = GetPartyId();
        if (!partyId.HasValue) return RedirectToAction("Index", "Login", new { area = "" });

        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden asignar candidatos durante una elección activa";
            return RedirectToAction("Index");
        }

        if (!ModelState.IsValid)
        {
            var candidatesResult = await _candidateService.GetByPartyIdAsync(partyId.Value);
            var positionsResult = await _positionService.GetAllAsync();
            ViewBag.Candidatos = candidatesResult.Data?.Where(c => c.IsActive).ToList() ?? new();
            ViewBag.Puestos = positionsResult.Data?.Where(p => p.IsActive).ToList() ?? new();
            return View(model);
        }

        var result = await _candidateService.AssignPositionAsync(model.CandidateId, model.PositionId, partyId.Value);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
        }
        else
        {
            TempData["Success"] = "Candidato asignado al puesto exitosamente";
        }

        return RedirectToAction("Index");
    }

    private async Task<string> SaveFotoAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "candidates");
        Directory.CreateDirectory(uploadsFolder);
        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return $"/images/candidates/{uniqueFileName}";
    }
}
