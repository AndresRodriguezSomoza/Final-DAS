using DAS_Final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DAS_Final.Controllers
{
    [Authorize]
    public class ControlHabitacion : Controller
    {
        private readonly OpHabitacion _operacionesHabitacion;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ControlHabitacion(IWebHostEnvironment hostingEnvironment)
        {
            _operacionesHabitacion = new OpHabitacion();
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var habitaciones = _operacionesHabitacion.ObtenerTodasHabitaciones();
            return View(habitaciones);
        }

        public IActionResult Details(int id)
        {
            var habitacion = _operacionesHabitacion.ObtenerHabitacionPorId(id);
            if (habitacion == null)
            {
                return NotFound();
            }
            return View(habitacion);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Habitacion habitacion, IFormFile imagenArchivo)
        {
            ModelState.Remove("Img");

            if (ModelState.IsValid)
            {
                if (_operacionesHabitacion.NumeroHabitacionExiste(habitacion.NumeroHabitacion))
                {
                    ModelState.AddModelError("NumeroHabitacion", "El número de habitación ya existe");
                    return View(habitacion);
                }

                // Validar que se subió una imagen
                if (imagenArchivo == null || imagenArchivo.Length == 0)
                {
                    ViewData["ErrorImagen"] = "La imagen es requerida";
                    return View(habitacion);
                }

                // Manejar la subida de la imagen
                var nombreArchivo = await GuardarImagen(imagenArchivo);
                habitacion.Img = nombreArchivo;

                if (_operacionesHabitacion.CrearHabitacion(habitacion))
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al crear la habitación");
            }
            return View(habitacion);
        }

        public IActionResult Edit(int id)
        {
            var habitacion = _operacionesHabitacion.ObtenerHabitacionPorId(id);
            if (habitacion == null)
            {
                return NotFound();
            }
            return View(habitacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Habitacion habitacion, IFormFile imagenArchivo)
        {
            if (id != habitacion.Id)
            {
                return NotFound();
            }

            // QUITAR la validación del campo Img
            ModelState.Remove("Img");

            if (ModelState.IsValid)
            {
                if (_operacionesHabitacion.NumeroHabitacionExiste(habitacion.NumeroHabitacion, habitacion.Id))
                {
                    ModelState.AddModelError("NumeroHabitacion", "El número de habitación ya existe");
                    return View(habitacion);
                }

                // Obtener la habitación actual para mantener la imagen si no se sube una nueva
                var habitacionActual = _operacionesHabitacion.ObtenerHabitacionPorId(id);

                if (habitacionActual == null)
                {
                    return NotFound();
                }

                // SIEMPRE mantener la imagen actual primero
                habitacion.Img = habitacionActual.Img;

                // Solo actualizar la imagen si se subió una nueva
                if (imagenArchivo != null && imagenArchivo.Length > 0)
                {
                    // Eliminar la imagen anterior si existe
                    if (!string.IsNullOrEmpty(habitacionActual.Img))
                    {
                        EliminarImagen(habitacionActual.Img);
                    }

                    var nombreArchivo = await GuardarImagen(imagenArchivo);
                    habitacion.Img = nombreArchivo;
                }

                // Intentar editar la habitación
                if (_operacionesHabitacion.EditarHabitacion(habitacion))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Error al actualizar la habitación");
                }
            }

            return View(habitacion);
        }

        public IActionResult Delete(int id)
        {
            var habitacion = _operacionesHabitacion.ObtenerHabitacionPorId(id);
            if (habitacion == null)
            {
                return NotFound();
            }
            return View(habitacion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var habitacion = _operacionesHabitacion.ObtenerHabitacionPorId(id);

            if (habitacion != null)
            {
                // Eliminar la imagen asociada
                if (!string.IsNullOrEmpty(habitacion.Img))
                {
                    EliminarImagen(habitacion.Img);
                }
            }

            if (_operacionesHabitacion.EliminarHabitacion(id))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(habitacion);
        }

        // Método para guardar la imagen en el folder Img
        private async Task<string> GuardarImagen(IFormFile imagenArchivo)
        {
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Img");

            // Crear el directorio si no existe
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generar un nombre único para el archivo
            var nombreUnico = Guid.NewGuid().ToString() + Path.GetExtension(imagenArchivo.FileName);
            var filePath = Path.Combine(uploadsFolder, nombreUnico);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imagenArchivo.CopyToAsync(fileStream);
            }

            return nombreUnico;
        }

        // Método para eliminar la imagen del folder Img
        private void EliminarImagen(string nombreArchivo)
        {
            if (!string.IsNullOrEmpty(nombreArchivo))
            {
                var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Img", nombreArchivo);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }
    }
}
