using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.ViewModels.Users;
using eVote360.Application.DTOs.Request;

namespace eVote360.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersController : Controller
{
    private readonly IUserService _service;
    private readonly IElectionService _electionService;
    private readonly IMapper _mapper;

    public UsersController(IUserService service, IElectionService electionService, IMapper mapper)
    {
        _service = service;
        _electionService = electionService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _service.GetAllAsync();
        var viewModels = _mapper.Map<List<UserVM>>(result.Data);

        var activeElection = await _electionService.GetActiveElectionAsync();
        ViewBag.HasActiveElection = activeElection.IsSuccess;

        return View(viewModels);
    }

    public async Task<IActionResult> Create()
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden crear usuarios durante una elección activa.";
            return RedirectToAction("Index");
        }
        return View(new UserVM());
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = _mapper.Map<UserCreateRequestDto>(model);
        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["Success"] = "Usuario creado exitosamente.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int id)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se pueden editar usuarios durante una elección activa.";
            return RedirectToAction("Index");
        }

        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess)
            return NotFound();

        var model = _mapper.Map<UserVM>(result.Data);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = _mapper.Map<UserUpdateRequestDto>(model);
        var result = await _service.UpdateAsync(dto);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        TempData["Success"] = "Usuario actualizado exitosamente.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmToggle(int id, bool activate)
    {
        var activeElection = await _electionService.GetActiveElectionAsync();
        if (activeElection.IsSuccess)
        {
            TempData["Error"] = "No se puede cambiar el estado de usuarios durante una elección activa.";
            return RedirectToAction("Index");
        }

        var result = await _service.GetByIdAsync(id);
        if (!result.IsSuccess) return NotFound();

        ViewBag.UserId = id;
        ViewBag.UserNombre = $"{result.Data!.Nombre} {result.Data.Apellido}";
        ViewBag.Activate = activate;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Toggle(int id)
    {
        var result = await _service.ToggleActiveAsync(id);
        if (!result.IsSuccess)
            TempData["Error"] = result.Message;
        else
            TempData["Success"] = "Estado del usuario actualizado.";
        return RedirectToAction("Index");
    }
}
