using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.ViewModels.Position;
using eVote360.Application.DTOs.Request;

namespace eVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class PositionsController : Controller
{
    private readonly IPositionService _service;
    private readonly IMapper _mapper;

    public PositionsController(IPositionService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _service.GetAllAsync();
        var viewModels = _mapper.Map<List<PositionListVM>>(result.Data);
        return View(viewModels);
    }

    public IActionResult Create()
    {
        return View(new PositionFormVM());
    }

    [HttpPost]
    public async Task<IActionResult> Create(PositionFormVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = _mapper.Map<PositionCreateRequest>(model);
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound();

        var model = _mapper.Map<PositionFormVM>(result.Data);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(PositionFormVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = _mapper.Map<PositionUpdateRequest>(model);
        var result = await _service.UpdateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Toggle(int id)
    {
        await _service.ToggleActiveAsync(id);
        return RedirectToAction("Index");
    }
}
