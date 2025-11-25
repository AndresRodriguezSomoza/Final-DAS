using DAS_Final.Models;
using Microsoft.AspNetCore.Mvc;

namespace DAS_Final.Controllers
{
    public class ControlUsuario : Controller
    {
        private readonly OpUsuario _operacionesUsuario;

        public ControlUsuario()
        {
            _operacionesUsuario = new OpUsuario();
        }

        public IActionResult Index()
        {
            var usuarios = _operacionesUsuario.ObtenerTodosUsuarios();
            return View(usuarios);
        }

        public IActionResult Details(int id)
        {
            var usuario = _operacionesUsuario.ObtenerUsuarioPorId(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (_operacionesUsuario.EmailExiste(usuario.Email))
                {
                    ModelState.AddModelError("Email", "El email ya está registrado");
                    return View(usuario);
                }

                if (_operacionesUsuario.CrearUsuario(usuario))
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al crear el usuario");
            }
            return View(usuario);
        }

        public IActionResult Edit(int id)
        {
            var usuario = _operacionesUsuario.ObtenerUsuarioPorId(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (_operacionesUsuario.EmailExiste(usuario.Email, usuario.Id))
                {
                    ModelState.AddModelError("Email", "El email ya está registrado");
                    return View(usuario);
                }

                if (_operacionesUsuario.EditarUsuario(usuario))
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Error al actualizar el usuario");
            }
            return View(usuario);
        }

        public IActionResult Delete(int id)
        {
            var usuario = _operacionesUsuario.ObtenerUsuarioPorId(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_operacionesUsuario.EliminarUsuario(id))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(_operacionesUsuario.ObtenerUsuarioPorId(id));
        }
    }
}
