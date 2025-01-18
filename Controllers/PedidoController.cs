using Mi_Negocio.Data;
using Mi_Negocio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading.Tasks;

public class PedidoController : Controller
{
    private readonly DataContext _context;

    public PedidoController(DataContext context)
    {
        _context = context;
    }

    // Página inicial para armar el pedido
    public async Task<IActionResult> Crear()
    {
        var productos = await _context.Productos.ToListAsync();
        return View(productos);
    }

    // Agregar producto al carrito
    [HttpPost]
    public IActionResult AgregarAlCarrito(int id_producto, int cantidad)
    {
        var carrito = HttpContext.Session.GetObjectFromJson<List<Detallespedido>>("Carrito") ?? new List<Detallespedido>();

        var producto = _context.Productos.Find(id_producto);

        if (producto != null)
        {
            var detalle = carrito.FirstOrDefault(d => d.id_producto == id_producto);
            if (detalle == null)
            {
                carrito.Add(new Detallespedido
                {
                    id_producto = producto.id_producto,
                    Producto = producto,
                    cantidad = cantidad,
                    total = producto.precio * cantidad
                });
            }
            else
            {
                detalle.Cantidad += cantidad;
                detalle.Subtotal += producto.precio * cantidad;
            }

            HttpContext.Session.SetObjectAsJson("Carrito", carrito);
        }

        return RedirectToAction("Crear");
    }

    // Ver carrito
    public IActionResult VerCarrito()
    {
        var carrito = HttpContext.Session.GetObjectFromJson<List<Detallespedido>>("Carrito") ?? new List<Detallespedido>();
        return View(carrito);
    }

    // Finalizar pedido
    [HttpPost]
    public async Task<IActionResult> FinalizarPedido(int direccionId)
    {
        var carrito = HttpContext.Session.GetObjectFromJson<List<Detallespedido>>("Carrito");

        if (carrito == null || !carrito.Any())
        {
            return RedirectToAction("Crear");
        }

        var pedido = new Pedido
        {
            Fecha = DateTime.Now,
            id_usuario = 1, // Supongamos un usuario autenticado con ID 1
            id_direccion = id_direccion,
            detalles = carrito,
            Total = carrito.Sum(d => d.Subtotal)
        };

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        HttpContext.Session.Remove("Carrito");

        return RedirectToAction("Confirmacion", new { id = pedido.id_pedido });
    }

    // Página de confirmación del pedido
    public async Task<IActionResult> Confirmacion(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.detalles)
            .ThenInclude(d => d.id_producto)
            .FirstOrDefaultAsync(p => p.id_pedido == id);

        return View(pedido);
    }
}