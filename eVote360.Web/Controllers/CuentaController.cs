using Microsoft.AspNetCore.Mvc;

namespace eVote360.Web.Controllers;

public class CuentaController : Controller
{
    public IActionResult Denegado()
    {
        return View();
    }

    public IActionResult IniciarSesion()
    {
        return RedirectToAction("Index", "Login");
    }
}
