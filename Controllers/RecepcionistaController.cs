using DAS_Final.Models;
using Microsoft.AspNetCore.Mvc;

namespace DAS_Final.Controllers
{
    public class RecepcionistaController : Controller
    {
        private readonly OpReservacion _opReservacion;
        private readonly OpUsuario _opUsuario;
        private readonly OpHabitacion _opHabitacion;

        public RecepcionistaController()
        {
            _opReservacion = new OpReservacion();
            _opUsuario = new OpUsuario();
            _opHabitacion = new OpHabitacion();
        }

        public IActionResult Index()
        {
            var reservaciones = _opReservacion.ObtenerTodasReservaciones();
            return View(reservaciones);
        }

        public IActionResult Create()
        {
            // EXACTAMENTE IGUAL que en tu ControlReservacion
            var usuarios = _opUsuario.ObtenerTodosUsuarios();
            var habitaciones = _opHabitacion.ObtenerTodasHabitaciones();

            if (usuarios == null || !usuarios.Any())
            {
                TempData["Error"] = "No hay usuarios disponibles";
            }

            if (habitaciones == null || !habitaciones.Any())
            {
                TempData["Error"] = "No hay habitaciones disponibles";
            }

            ViewBag.Usuarios = usuarios ?? new List<Usuario>();
            ViewBag.Habitaciones = habitaciones ?? new List<Habitacion>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reservacion reservacion)
        {
            // EXACTAMENTE IGUAL que en tu ControlReservacion
            ModelState.Remove("TotalHabitacion");
            ModelState.Remove("Usuario");
            ModelState.Remove("Habitacion");

            if (!ModelState.IsValid)
            {
                ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                return View(reservacion);
            }

            if (reservacion.UsuarioId <= 0 || reservacion.HabitacionId <= 0)
            {
                ModelState.AddModelError("", "Debe seleccionar un usuario y una habitación válidos");
                ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                return View(reservacion);
            }

            if (reservacion.FechaSalida <= reservacion.FechaEntrada)
            {
                ModelState.AddModelError("FechaSalida", "La fecha de salida debe ser posterior a la fecha de entrada");
                ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                return View(reservacion);
            }

            var habitacion = _opHabitacion.ObtenerHabitacionPorId(reservacion.HabitacionId);
            if (habitacion == null)
            {
                ModelState.AddModelError("HabitacionId", "Habitación no encontrada");
                ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                return View(reservacion);
            }

            var dias = (reservacion.FechaSalida - reservacion.FechaEntrada).Days;
            if (dias <= 0)
            {
                ModelState.AddModelError("", "Las fechas deben ser válidas");
                ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                return View(reservacion);
            }

            reservacion.TotalHabitacion = habitacion.PrecioNoche * dias;

            try
            {
                if (_opReservacion.CrearReservacion(reservacion))
                {
                    TempData["Success"] = "Reservación creada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo crear la reservación en la base de datos");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
            }

            ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
            ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
            return View(reservacion);
        }

        public IActionResult Details(int id)
        {
            var reservacion = _opReservacion.ObtenerReservacionPorId(id);
            if (reservacion == null)
            {
                return NotFound();
            }
            return View(reservacion);
        }

        public IActionResult Edit(int id)
        {
            var reservacion = _opReservacion.ObtenerReservacionPorId(id);
            if (reservacion == null)
            {
                return NotFound();
            }

            // EXACTAMENTE IGUAL que en tu ControlReservacion
            ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
            ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
            return View(reservacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Reservacion reservacion)
        {
            if (id != reservacion.Id)
            {
                return NotFound();
            }

            // EXACTAMENTE IGUAL que en tu ControlReservacion
            ModelState.Remove("TotalHabitacion");
            ModelState.Remove("Usuario");
            ModelState.Remove("Habitacion");

            if (ModelState.IsValid)
            {
                if (reservacion.FechaSalida <= reservacion.FechaEntrada)
                {
                    ModelState.AddModelError("FechaSalida", "La fecha de salida debe ser posterior a la fecha de entrada");
                    ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                    ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                    return View(reservacion);
                }

                var habitacion = _opHabitacion.ObtenerHabitacionPorId(reservacion.HabitacionId);
                if (habitacion == null)
                {
                    ModelState.AddModelError("HabitacionId", "Habitación no encontrada");
                    ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                    ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                    return View(reservacion);
                }

                var dias = (reservacion.FechaSalida - reservacion.FechaEntrada).Days;
                reservacion.TotalHabitacion = habitacion.PrecioNoche * dias;

                if (_opReservacion.EditarReservacion(reservacion))
                {
                    TempData["Success"] = "Reservación actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al actualizar la reservación");
            }

            ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
            ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
            return View(reservacion);
        }

        // Funciones específicas del recepcionista (sin Delete)
        public IActionResult CheckIn(int id)
        {
            var reservacion = _opReservacion.ObtenerReservacionPorId(id);
            if (reservacion == null)
            {
                return NotFound();
            }

            reservacion.Estatus = "activa";
            if (_opReservacion.EditarReservacion(reservacion))
            {
                TempData["Success"] = "Check-in realizado exitosamente";
            }
            else
            {
                TempData["Error"] = "Error al realizar el check-in";
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult CheckOut(int id)
        {
            var reservacion = _opReservacion.ObtenerReservacionPorId(id);
            if (reservacion == null)
            {
                return NotFound();
            }

            reservacion.Estatus = "completada";
            if (_opReservacion.EditarReservacion(reservacion))
            {
                TempData["Success"] = "Check-out realizado exitosamente";
            }
            else
            {
                TempData["Error"] = "Error al realizar el check-out";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
