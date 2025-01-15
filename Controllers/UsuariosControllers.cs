
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Mi_Negocio.Models;
using Mi_Negocio.Repositorios;
using System.IO;
using System.Threading.Tasks;

namespace Mi_Negocio.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly RepositorioUsuarios _repositorio;
        private readonly IWebHostEnvironment _environment;

        public UsuariosController(RepositorioUsuarios repositorio, IWebHostEnvironment environment)
        {
            _repositorio = repositorio;
            _environment = environment;
        }

        // Listar usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _repositorio.ObtenerUsuariosAsync();
            return View(usuarios);
        }

        // Crear usuario - GET
        public IActionResult Crear()
        {
            return View(new Usuario());
        }

        // Crear usuario - POST
        [HttpPost]
        
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (usuario.AvatarFile != null)
                {
                    usuario.Avatar = GuardarAvatar(usuario.AvatarFile);
                }
                else
                {
                    usuario.Avatar = "/avatars/avatar_0.png"; // Avatar predeterminado
                }

                await _repositorio.AgregarUsuarioAsync(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // Editar usuario - GET
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _repositorio.ObtenerUsuarioPorIdAsync(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // Editar usuario - POST
     [HttpPost]
public async Task<IActionResult> Editar(Usuario usuario)
{
    if (ModelState.IsValid)
    {
        try
        {
            string wwwPath = _environment.WebRootPath;
            string path = Path.Combine(wwwPath, "avatars");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Procesar el nuevo avatar si se carga uno
            if (usuario.AvatarFile != null)
            {
                // Generar un nuevo nombre para el avatar
                string fileName = "avatar_" + Guid.NewGuid().ToString() + Path.GetExtension(usuario.AvatarFile.FileName);
                string fullPath = Path.Combine(path, fileName);
                string nuevaRutaAvatar = $"/avatars/{fileName}";

                // Guardar el nuevo avatar
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await usuario.AvatarFile.CopyToAsync(stream);
                }

                // Eliminar el avatar anterior si no es el predeterminado
                if (!string.IsNullOrEmpty(usuario.Avatar) && usuario.Avatar != "/avatars/default.jpg")
                {
                    string rutaAvatarAnterior = Path.Combine(wwwPath, usuario.Avatar.TrimStart('/'));
                    if (System.IO.File.Exists(rutaAvatarAnterior))
                    {
                        System.IO.File.Delete(rutaAvatarAnterior);
                        Console.WriteLine($"Avatar anterior eliminado: {rutaAvatarAnterior}");
                    }
                }

                // Actualizar la propiedad 'Avatar' con la nueva ruta
                usuario.Avatar = nuevaRutaAvatar;
            }
            else
            {
                // Si no se carga un nuevo avatar, conservar el actual
                Console.WriteLine("No se cargó un nuevo avatar. Se conserva el existente.");
            }

            // Actualizar el usuario en la base de datos
            var resultado = _repositorio.EditarUsuario(usuario);

            if (resultado) // Verificar si la operación fue exitosa
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "No se pudo actualizar el usuario en la base de datos.");
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Ocurrió un error: " + ex.Message);
        }
    }

    // Si llega aquí, algo falló
    return View(usuario);
}

        // Eliminar usuario - POST
        [HttpPost]
        
        public IActionResult Eliminar(int id)
        {
            _repositorio.EliminarUsuario(id);
            return RedirectToAction(nameof(Index));
        }

        // Dar de alta a un usuario
        [HttpPost]
        
        public IActionResult DarAlta(int id)
        {
            _repositorio.DarAlta(id);
            return RedirectToAction(nameof(Index));
        }

        // Método privado para guardar el avatar
        private string GuardarAvatar(IFormFile avatarFile)
        {
            string carpetaAvatares = Path.Combine(_environment.WebRootPath, "avatars");
            if (!Directory.Exists(carpetaAvatares))
            {
                Directory.CreateDirectory(carpetaAvatares);
            }

            string archivoAvatar = Path.GetFileNameWithoutExtension(avatarFile.FileName)
                                   + "_" + Path.GetRandomFileName().Substring(0, 8)
                                   + Path.GetExtension(avatarFile.FileName);
            string rutaCompleta = Path.Combine(carpetaAvatares, archivoAvatar);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                avatarFile.CopyTo(stream);
            }

            return $"/avatars/{archivoAvatar}";
        }
    }
}
