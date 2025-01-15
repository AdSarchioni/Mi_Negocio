
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Mi_Negocio.Models;
using Mi_Negocio.Repositorios;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Mi_Negocio.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly RepositorioUsuarios _repositorio;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration configuration;

        public UsuariosController(RepositorioUsuarios repositorio, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _repositorio = repositorio;
            _environment = environment;
            this.configuration = configuration;
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
        try
        {
            // Hashear la contraseña
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: usuario.Password,
                salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));

            usuario.Password = hashedPassword;

            // Procesar el avatar
            if (usuario.AvatarFile != null)
            {
                usuario.Avatar = GuardarAvatar(usuario.AvatarFile);
            }
            else
            {
                usuario.Avatar = "/avatars/avatar_0.png"; // Avatar predeterminado
            }

            // Guardar el usuario en la base de datos
            await _repositorio.AgregarUsuarioAsync(usuario);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // Manejar errores
            ModelState.AddModelError("", "Ocurrió un error al crear el usuario: " + ex.Message);
        }
    }

    // Si llega aquí, algo falló
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
            // Recuperar el usuario actual desde la base de datos
            var usuarioExistente = await _repositorio.ObtenerUsuarioPorIdAsync(usuario.Id_Usuario);
            if (usuarioExistente == null)
            {
                ModelState.AddModelError("", "El usuario no existe.");
                return View(usuario);
            }

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
                if (!string.IsNullOrEmpty(usuarioExistente.Avatar) && usuarioExistente.Avatar != "/avatars/default.jpg")
                {
                    string rutaAvatarAnterior = Path.Combine(wwwPath, usuarioExistente.Avatar.TrimStart('/'));
                    if (System.IO.File.Exists(rutaAvatarAnterior))
                    {
                        System.IO.File.Delete(rutaAvatarAnterior);
                        Console.WriteLine($"Avatar anterior eliminado: {rutaAvatarAnterior}");
                    }
                }

                // Actualizar la propiedad 'Avatar' con la nueva ruta
                usuarioExistente.Avatar = nuevaRutaAvatar;
            }

            // Solo actualizar campos no vacíos
            if (!string.IsNullOrEmpty(usuario.Nombre))
            {
                usuarioExistente.Nombre = usuario.Nombre;
            }
            if (!string.IsNullOrEmpty(usuario.Email))
            {
                usuarioExistente.Email = usuario.Email;
            }
            if (!string.IsNullOrEmpty(usuario.Password))
            {
                // Hashear la nueva contraseña si se proporciona
                string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: usuario.Password,
                    salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                usuarioExistente.Password = hashedPassword;
            }

            // Actualizar el usuario en la base de datos
            var resultado = _repositorio.EditarUsuario(usuarioExistente);

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
