using Mi_Negocio.Data;
using Mi_Negocio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mi_Negocio.Controllers;
public class PedidosController : Controller
{
    private readonly DataContext _context;

    public PedidosController(DataContext context)
    {
        _context = context;
    }
[HttpGet]
public IActionResult Index()
{
    var pedidos = _context.Pedidos
        .Include(p => p.Detallespedido)
        .Include(p => p.Usuario)
        .ToList();

    var viewModel = new PedidosIndexViewModel
    {
        Pedidos = pedidos,
        EstadoSeleccionado = "Todos"
    };

    return View(viewModel);
}

    // Acción para listar pedidos filtrados por estado
   [HttpPost]
public IActionResult Filtrar(PedidosIndexViewModel model)
{
    // Verifica el valor del estado
    string estado = model.EstadoSeleccionado ?? "Enviados";
Console.WriteLine("Estado seleccionado: " + model.EstadoSeleccionado);
    // Obtener todos los pedidos
    var pedidosQuery = _context.Pedidos
        .Include(p => p.Detallespedido)
        .Include(p => p.Usuario)
        .AsQueryable();

    // Filtrar pedidos según el estado
    if (estado != "Todos")
    {
        int estadoInt = estado switch
        {
            "Recibidos" => 1,
            "En Proceso" => 2,
            "Enviados" => 3,
            _ => 0
        };

        pedidosQuery = pedidosQuery.Where(p => p.estado == estadoInt);
    }

    // Obtener la lista de pedidos
    var pedidos = pedidosQuery.ToList();

    // Preparar el ViewModel para la vista
    model.Pedidos = pedidos;

    return View("Index", model);
}

public IActionResult Detalles(int id)
{
    var pedido = _context.Pedidos
        .Include(p => p.Detallespedido)
        .ThenInclude(dp => dp.Producto)
        .FirstOrDefault(p => p.pedidoId == id);

    if (pedido == null)
    {
        return NotFound("El pedido no existe.");
    }

    return View(pedido);
}
[HttpPost]
public IActionResult ActualizarEstado(int pedidoId, int estado)
{
    // Buscar el pedido en la base de datos
    var pedido = _context.Pedidos.FirstOrDefault(p => p.pedidoId == pedidoId);

    if (pedido == null)
    {
        return NotFound("El pedido no existe.");
    }

    // Actualizar el estado del pedido
    pedido.estado = estado;

    // Guardar los cambios en la base de datos
    _context.SaveChanges();

    // Redirigir nuevamente a la vista de detalles
    return RedirectToAction("Detalles", new { id = pedidoId });
}
[HttpGet]
public IActionResult IndexFiltrado()
{
    // Obtener todos los pedidos
    var pedidos = _context.Pedidos
        .Include(p => p.Detallespedido)
        .Include(p => p.Usuario)
        .ToList();

    // Obtener todos los usuarios para el filtro
    var usuarios = _context.Usuarios
        .Select(u => new { u.usuarioId, NombreCompleto = $"{u.nombre} {u.apellido}" })
        .ToList<dynamic>();

    var viewModel = new PedidosIndexViewModel
    {
        Pedidos = pedidos,
        EstadoSeleccionado = "Todos",
        Usuarios = usuarios,
        UsuarioIdSeleccionado = null
    };

    return View("IndexFiltrado", viewModel);
}

[HttpPost]
public IActionResult FiltrarPorUsuarioYPedido(PedidosIndexViewModel model)
{
    // Obtener los pedidos según filtros
    var pedidosQuery = _context.Pedidos
        .Include(p => p.Detallespedido)
        .Include(p => p.Usuario)
        .AsQueryable();

    // Filtrar por estado
    if (model.EstadoSeleccionado != "Todos")
    {
        int estadoInt = model.EstadoSeleccionado switch
        {
            "Recibidos" => 1,
            "En Proceso" => 2,
            "Enviados" => 3,
            _ => 0
        };

        pedidosQuery = pedidosQuery.Where(p => p.estado == estadoInt);
    }

    // Filtrar por usuario
    if (model.UsuarioIdSeleccionado.HasValue)
    {
        pedidosQuery = pedidosQuery.Where(p => p.usuarioId == model.UsuarioIdSeleccionado.Value);
    }

    var pedidos = pedidosQuery.ToList();

    // Mantener la lista de usuarios para el filtro
    var usuarios = _context.Usuarios
        .Select(u => new { u.usuarioId, NombreCompleto = $"{u.nombre} {u.apellido}" })
        .ToList<dynamic>();

    model.Pedidos = pedidos;
    model.Usuarios = usuarios;

    return View("IndexFiltrado", model);
}


}
