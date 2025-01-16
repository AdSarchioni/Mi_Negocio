using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Mi_Negocio.Models;
using Mi_Negocio.Repositorios;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Mi_Negocio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration; 
   
    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
  public async Task<IActionResult> Login(LoginView usuariologin)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                else
                {
                    if (usuariologin == null)
                    {
                        usuariologin = new LoginView();
                    }
                    var Mensaje = "";
                    RepositorioUsuarios ru = new RepositorioUsuarios();

                    // Usamos _configuration para obtener el valor de "Salt"
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: usuariologin.Password,
                        salt: System.Text.Encoding.ASCII.GetBytes(_configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    var usuario = ru.ObtenerUsuarioLogin(usuariologin.Email, hashed);
                    if (usuario == null || usuario.Password != hashed)
                    {
                        Mensaje = "El usuario o la contraseña son incorrectos";
                        if (usuariologin.Password == "")
                        {
                            Mensaje = "";
                        }

                        ViewBag.Mensaje = Mensaje;
                        return View();
                    }

                    var claims = new List<Claim>{

                        new Claim(ClaimTypes.Name, usuario.ToString()),
                        new Claim(ClaimTypes.PrimarySid, usuario.Id_Usuario.ToString()),
                        new Claim(ClaimTypes.UserData, usuario.Avatar ?? ""),
                        new Claim(ClaimTypes.Email, usuario.Email),
                        new Claim(ClaimTypes.Role, usuario.RolNombre),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
        public async Task<ActionResult> salir()
        {

            await HttpContext.SignOutAsync(
               CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
