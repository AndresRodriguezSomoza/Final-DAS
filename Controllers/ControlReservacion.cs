using DAS_Final.Models;
using Microsoft.AspNetCore.Mvc;

namespace DAS_Final.Controllers
{
    public class ControlReservacion : Controller
    {
        private readonly OpReservacion _opReservacion;
        private readonly OpUsuario _opUsuario;
        private readonly OpHabitacion _opHabitacion;

        public ControlReservacion()
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
            ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
            ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reservacion reservacion)
        {
            // Quitar validación del Total
            ModelState.Remove("TotalHabitacion");

            if (ModelState.IsValid)
            {
                // Validar fechas
                if (reservacion.FechaSalida <= reservacion.FechaEntrada)
                {
                    ModelState.AddModelError("FechaSalida", "La fecha de salida debe ser posterior a la fecha de entrada");
                    ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                    ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                    return View(reservacion);
                }

                // Obtener habitación para calcular precio
                var habitacion = _opHabitacion.ObtenerHabitacionPorId(reservacion.HabitacionId);
                if (habitacion == null)
                {
                    ModelState.AddModelError("HabitacionId", "Habitación no encontrada");
                    ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                    ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                    return View(reservacion);
                }

                // Calcular total
                var dias = (reservacion.FechaSalida - reservacion.FechaEntrada).Days;
                reservacion.TotalHabitacion = habitacion.PrecioNoche * dias;

                // Validar disponibilidad
                if (!_opReservacion.HabitacionDisponible(reservacion.HabitacionId, reservacion.FechaEntrada, reservacion.FechaSalida))
                {
                    ModelState.AddModelError("HabitacionId", "La habitación no está disponible para las fechas seleccionadas");
                    ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                    ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                    return View(reservacion);
                }

                if (_opReservacion.CrearReservacion(reservacion))
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al crear la reservación");
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

            ModelState.Remove("TotalHabitacion");

            if (ModelState.IsValid)
            {
                // Validar fechas
                if (reservacion.FechaSalida <= reservacion.FechaEntrada)
                {
                    ModelState.AddModelError("FechaSalida", "La fecha de salida debe ser posterior a la fecha de entrada");
                    ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                    ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                    return View(reservacion);
                }

                // Obtener habitación para calcular precio
                var habitacion = _opHabitacion.ObtenerHabitacionPorId(reservacion.HabitacionId);
                if (habitacion == null)
                {
                    ModelState.AddModelError("HabitacionId", "Habitación no encontrada");
                    ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                    ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                    return View(reservacion);
                }

                // Calcular total
                var dias = (reservacion.FechaSalida - reservacion.FechaEntrada).Days;
                reservacion.TotalHabitacion = habitacion.PrecioNoche * dias;

                // Validar disponibilidad
                if (!_opReservacion.HabitacionDisponible(reservacion.HabitacionId, reservacion.FechaEntrada, reservacion.FechaSalida, reservacion.Id))
                {
                    ModelState.AddModelError("HabitacionId", "La habitación no está disponible para las fechas seleccionadas");
                    ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
                    ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
                    return View(reservacion);
                }

                if (_opReservacion.EditarReservacion(reservacion))
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al actualizar la reservación");
            }

            ViewBag.Usuarios = _opUsuario.ObtenerTodosUsuarios();
            ViewBag.Habitaciones = _opHabitacion.ObtenerTodasHabitaciones();
            return View(reservacion);
        }

        public IActionResult Delete(int id)
        {
            var reservacion = _opReservacion.ObtenerReservacionPorId(id);
            if (reservacion == null)
            {
                return NotFound();
            }
            return View(reservacion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_opReservacion.EliminarReservacion(id))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(_opReservacion.ObtenerReservacionPorId(id));
        }
    }
}
