using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.ViewModels.Citizens;
using eVote360.Application.DTOs.Request;

namespace eVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class CitizensController : Controller
{
    private readonly ICitizenService _service;
    private readonly IElectionService _electionService;
    private readonly IMapper _mapper;

    public CitizensController(ICitizenService service, IElectionService electionService, IMapper mapper)
    {
        _service = service;
        _electionService = electionService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _service.GetAllAsync();
        var viewModels = _mapper.Map<List<CitizenVM>>(result.Data);
        
        var activeElection = await _electionService.GetActiveElectionAsync();
        ViewBag.HasActiveElection = activeElection.IsSuccess;
        
        return View(viewModels);
    }

    public async Task<IActionResult> Create()
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden crear ciudadanos mientras hay una elección activa";
            return RedirectToAction("Index");
        }
        return View(new CitizenVM());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CitizenVM model)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden crear ciudadanos mientras hay una elección activa";
            return RedirectToAction("Index");
        }

        if (!ModelState.IsValid)
            return View(model);

        var dto = _mapper.Map<CitizenCreateRequestDto>(model);
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["Success"] = "Ciudadano creado exitosamente";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden editar ciudadanos mientras hay una elección activa";
            return RedirectToAction("Index");
        }

        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound();

        var model = _mapper.Map<CitizenVM>(result.Data);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CitizenVM model)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden editar ciudadanos mientras hay una elección activa";
            return RedirectToAction("Index");
        }

        if (!ModelState.IsValid)
            return View(model);

        var dto = _mapper.Map<CitizenUpdateRequestDto>(model);
        var result = await _service.UpdateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["Success"] = "Ciudadano actualizado exitosamente";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Toggle(int id)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se puede cambiar el estado de ciudadanos mientras hay una elección activa";
            return RedirectToAction("Index");
        }

        await _service.ToggleActiveAsync(id);
        TempData["Success"] = "Estado del ciudadano actualizado";
        return RedirectToAction("Index");
    }
}
