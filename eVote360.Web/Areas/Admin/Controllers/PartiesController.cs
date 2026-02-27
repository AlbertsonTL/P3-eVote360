using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.ViewModels.Party;
using eVote360.Application.DTOs.Request;

namespace eVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class PartiesController : Controller
{
    private readonly IPartyService _service;
    private readonly IElectionService _electionService;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _env;

    public PartiesController(IPartyService service, IElectionService electionService, IMapper mapper, IWebHostEnvironment env)
    {
        _service = service;
        _electionService = electionService;
        _mapper = mapper;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _service.GetAllAsync();
        var viewModels = _mapper.Map<List<PartyListVM>>(result.Data);
        
        var activeElection = await _electionService.GetActiveElectionAsync();
        ViewBag.HasActiveElection = activeElection.IsSuccess;
        
        return View(viewModels);
    }

    public async Task<IActionResult> Create()
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden crear partidos mientras hay una elección activa";
            return RedirectToAction("Index");
        }
        return View(new PartyFormVM());
    }

    [HttpPost]
    public async Task<IActionResult> Create(PartyFormVM model)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden crear partidos mientras hay una elección activa";
            return RedirectToAction("Index");
        }

        if (!ModelState.IsValid)
            return View(model);

        string logoPath = "/images/logos/default.png";
        if (model.Logo != null)
        {
            logoPath = await SaveLogoAsync(model.Logo);
        }

        var dto = _mapper.Map<PartyCreateRequest>(model);
        var result = await _service.CreateAsync(dto, logoPath);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["Success"] = "Partido creado exitosamente";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden editar partidos mientras hay una elección activa";
            return RedirectToAction("Index");
        }

        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound();

        var model = _mapper.Map<PartyFormVM>(result.Data);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(PartyFormVM model)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden editar partidos mientras hay una elección activa";
            return RedirectToAction("Index");
        }

        if (!ModelState.IsValid)
            return View(model);

        string? logoPath = null;
        if (model.Logo != null)
        {
            logoPath = await SaveLogoAsync(model.Logo);
        }

        var dto = _mapper.Map<PartyUpdateRequest>(model);
        var result = await _service.UpdateAsync(dto, logoPath);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["Success"] = "Partido actualizado exitosamente";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Toggle(int id)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se puede cambiar el estado de partidos mientras hay una elección activa";
            return RedirectToAction("Index");
        }

        await _service.ToggleActiveAsync(id);
        TempData["Success"] = "Estado del partido actualizado";
        return RedirectToAction("Index");
    }

    private async Task<string> SaveLogoAsync(IFormFile file)
    {
        var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "logos");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/images/logos/{uniqueFileName}";
    }
}
