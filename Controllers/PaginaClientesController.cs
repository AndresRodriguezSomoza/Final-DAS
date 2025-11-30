using DAS_Final.Models; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DAS_Final.Controllers
{
    public class PaginaClientesController : Controller
    {
        // --- 1. DECLARACIÓN DE VARIABLES 
        private readonly OpHabitacion _opHabitacion;
        private readonly OpReservacion _opReservacion;

        // --- 2. CONSTRUCTOR ---
        public PaginaClientesController()
        {
           
            _opHabitacion = new OpHabitacion();
            _opReservacion = new OpReservacion();
        }

        // --- 3. ACCIONES (MÉTODOS) ---

        // GET: PaginaClientes/Index
        public IActionResult Index()
        {
            var habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
            // Filtro opcional si quieres solo las disponibles
            var disponibles = habitaciones.Where(h => h.Estatus == "Disponible" || h.Estatus == "disponible").ToList();
            return View(disponibles);
        }

        // GET: PaginaClientes/Rooms
        public IActionResult Rooms()
        {
            var habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
            return View(habitaciones);
        }

        // GET: Mostrar formulario de reserva
        [Authorize]
        public IActionResult Reservar(int id)
        {
            var habitacion = _opHabitacion.ObtenerHabitacionPorId(id);
            if (habitacion == null) return NotFound();

            var reserva = new Reservacion
            {
                HabitacionId = id,
                Habitacion = habitacion,
                FechaEntrada = DateTime.Today,
                FechaSalida = DateTime.Today.AddDays(1)
            };

            return View(reserva);
        }

        //  Guardar la reserva
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Reservar(Reservacion reservacion)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Auth");
            }

            reservacion.UsuarioId = int.Parse(userIdStr);
            reservacion.Estatus = "pendiente";

            // Limpiamos validaciones que no aplican en este formulario
            ModelState.Remove("Usuario");
            ModelState.Remove("Habitacion");
            ModelState.Remove("TotalHabitacion");

            if (ModelState.IsValid)
            {
                if (reservacion.FechaSalida <= reservacion.FechaEntrada)
                {
                    ViewBag.Error = "La fecha de salida debe ser posterior a la entrada.";
                    reservacion.Habitacion = _opHabitacion.ObtenerHabitacionPorId(reservacion.HabitacionId);
                    return View(reservacion);
                }

                if (_opReservacion.HayConflictoDeFechas(
                    reservacion.HabitacionId,
                    reservacion.FechaEntrada,
                    reservacion.FechaSalida))
                {
                    ViewBag.Error = "Esta habitación ya está reservada en esas fechas.";
                    reservacion.Habitacion = _opHabitacion.ObtenerHabitacionPorId(reservacion.HabitacionId);
                    return View(reservacion);
                }

                if (_opReservacion.CrearReservacion(reservacion))
                {
                    TempData["Success"] = "¡Reserva realizada con éxito!";
                    return RedirectToAction("Index");
                }
            }

            reservacion.Habitacion = _opHabitacion.ObtenerHabitacionPorId(reservacion.HabitacionId);
            return View(reservacion);
        }
    }
}
