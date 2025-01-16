

using System.Security.Claims;
using Mi_Negocio.Data;
using Mi_Negocio.Repositorios;
using Microsoft.EntityFrameworkCore;
using MySql.Data;


var builder = WebApplication.CreateBuilder(args);

// Agrega los servicios para controladores con vistas
builder.Services.AddControllersWithViews();

// Registra el DbContext para la conexión a la base de datos
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Registra el repositorio de productos
 builder.Services.AddScoped<RepositorioProductos>(); // Esto es clave
 builder.Services.AddScoped<RepositorioUsuarios>();

builder.Services.AddControllers();  
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
