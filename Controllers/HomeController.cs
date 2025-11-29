using DAS_Final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace DAS_Final.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = User.Identity?.Name ?? "Usuario";
            ViewBag.UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Usuario";
            return View();
        }

        // Agrega esta acción para redirigir a los CRUD
        public IActionResult RedirectToCrud(string crudName)
        {
            return crudName?.ToLower() switch
            {
                "usuarios" => RedirectToAction("Index", "ControlUsuario"),
                "habitaciones" => RedirectToAction("Index", "ControlHabitacion"),
                "reservaciones" => RedirectToAction("Index", "ControlReservacion"),
                _ => RedirectToAction("Index")
            };
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
