using Microsoft.AspNetCore.Mvc;
using eVote360.Application.Abstractions.Services;
using eVote360.Web.Models;

namespace eVote360.Web.Controllers;

public class LoginController : Controller
{
    private readonly IAuthService _authService;
    private readonly IPartyAssignmentService _partyAssignmentService;
    private readonly IUserSession _userSession;

    public LoginController(IAuthService authService, IPartyAssignmentService partyAssignmentService, IUserSession userSession)
    {
        _authService = authService;
        _partyAssignmentService = partyAssignmentService;
        _userSession = userSession;
    }

    [HttpGet]
    public IActionResult Index()
    {
        // If already logged in, redirect to appropriate area
        var role = HttpContext.Session.GetString("UserRole");
        if (role == "Administrador")
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        if (role == "Dirigente")
            return RedirectToAction("Index", "Home", new { area = "Dirigente" });

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.AuthenticateAsync(model.Usuario, model.Password);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return View(model);
        }

        var user = result.Data!;
        _userSession.SetUser(user);
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserRole", user.Rol);
        HttpContext.Session.SetString("UserNombre", $"{user.Nombre} {user.Apellido}");

        // If dirigente, find and store party assignment
        if (user.Rol == "Dirigente")
        {
            var assignments = await _partyAssignmentService.GetAllAsync();
            var myAssignment = assignments.Data?.FirstOrDefault(a => a.UsuarioId == user.Id);
            if (myAssignment != null)
            {
                HttpContext.Session.SetInt32("PartyId", myAssignment.PartyId);
            }
            return RedirectToAction("Index", "Home", new { area = "Dirigente" });
        }

        return RedirectToAction("Index", "Home", new { area = "Admin" });
    }

    public IActionResult Logout()
    {
        _userSession.Clear();
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}
