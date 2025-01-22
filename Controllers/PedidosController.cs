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

    // Acción para listar pedidos filtrados por estado
    public IActionResult Index(string estado = "Todos")
    {
        // Obtener todos los pedidos
        var pedidosQuery = _context.Pedidos
            .Include(p => p.Detallespedido) // Cargar los detalles de los pedidos
            .Include(p => p.Usuario)       // Cargar la información del usuario
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
        var viewModel = new PedidosIndexViewModel
        {
            Pedidos = pedidos,
            EstadoSeleccionado = estado
        };

        return View(viewModel);
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


}
