

using System.Security.Claims;
using Mi_Negocio.Data;
using Mi_Negocio.Repositorios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MySql.Data;


var builder = WebApplication.CreateBuilder(args);
// Agregar soporte para sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true; // Seguridad de la cookie
    options.Cookie.IsEssential = true; // Obligatorio para cumplimiento de RGPD
});
// Agrega los servicios para controladores con vistas
builder.Services.AddControllersWithViews();

// Registra el DbContext para la conexión a la base de datos
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Registra el repositorio de productos
 builder.Services.AddScoped<RepositorioProductos>(); // Esto es clave
 builder.Services.AddScoped<RepositorioUsuarios>();


// Configura la autenticación y autorización
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home";
        options.AccessDeniedPath = "/Home";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5); // Tiempo de expiración
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Cliente", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador", "Cliente"));
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Registrar el middleware de sesiones
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
