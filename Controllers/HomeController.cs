using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mi_Negocio.Models;
using Mi_Negocio.Repositorios;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Mi_Negocio.Controllers
{
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration; 
    private readonly RepositorioUsuarios _repositorioUsuarios;
   
    public HomeController(ILogger<HomeController> logger, IConfiguration configuration, RepositorioUsuarios repositorioUsuarios)
    {
        _logger = logger;
        _configuration = configuration;
        _repositorioUsuarios = repositorioUsuarios;
    }




    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
  // Método GET para mostrar la vista de login
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginView()); // Retorna la vista vacía
    }

    // Método POST para procesar los datos de login
    [HttpPost]
    public async Task<IActionResult> Login(LoginView usuariologin)
    {


        Console.WriteLine("Entro al Login   =   " + usuariologin.Email + usuariologin.Password);
        try
        {
            if (!ModelState.IsValid)
            {
                return View(usuariologin); // Retorna la vista con los datos ingresados y errores de validación
            }

            var mensaje = "";
         Console.WriteLine("Entro al Login2   =   " + usuariologin.Email + usuariologin.Password);
            // Generar el hash de la contraseña ingresada
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: usuariologin.Password,
                salt: System.Text.Encoding.ASCII.GetBytes(_configuration["Salt"]),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));

            // Validar las credenciales del usuario
        var usuario = await _repositorioUsuarios.ObtenerUsuarioLoginAsync(usuariologin.Email, hashed);      

         Console.WriteLine("Entro al Login4   =   " + usuario.email + usuario.password);   
            if (usuario == null || usuario.password != hashed)
            {
                mensaje = "El usuario o la contraseña son incorrectos";
                ViewBag.Mensaje = mensaje;
                return View(usuariologin);
            }

            // Crear los claims para el usuario autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.ToString()),
                new Claim(ClaimTypes.PrimarySid, usuario.id_usuario.ToString()),
                new Claim(ClaimTypes.UserData, usuario.avatar ?? ""),
                new Claim(ClaimTypes.Email, usuario.email),
                new Claim(ClaimTypes.Role, usuario.rolnombre),

            };
  
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(usuariologin);
        }
    }
}
}
