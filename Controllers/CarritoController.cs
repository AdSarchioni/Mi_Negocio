using Mi_Negocio.Data;
using Mi_Negocio.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class CarritoController : Controller
{
    private readonly DataContext _context;

    public CarritoController(DataContext context)
    {
        _context = context;
    }

    // Acción para mostrar todos los productos disponibles
    public IActionResult SeleccionarProductos()
    {
        var productos = _context.Productos.ToList(); // Obtienes todos los productos
        return View(productos);
    }

    // Acción para agregar un producto al carrito
    [HttpPost]
    public IActionResult AgregarProductoAlCarrito(int idProducto, int cantidad)
    {
        var producto = _context.Productos.Find(idProducto);
        if (producto == null)
        {
            return NotFound("Producto no encontrado.");
        }

        // Obtener el carrito de la sesión
        var carrito = ObtenerCarrito();

        // Si el producto ya está en el carrito, aumentamos la cantidad
        var detalleExistente = carrito.Items.FirstOrDefault(item => item.id_producto == idProducto);
        if (detalleExistente != null)
        {
            detalleExistente.cantidad += cantidad;
        }
        else
        {
            // Si no está en el carrito, lo agregamos
            carrito.Items.Add(new Detallespedido
            {
                id_producto = idProducto,
            
                cantidad = cantidad,
                total = producto.precio * cantidad
            });
        }

        // Guardamos el carrito en la sesión
        GuardarCarrito(carrito);

        return RedirectToAction("SeleccionarProductos");
    }

    // Acción para finalizar la compra y guardar en Detallespedido
   [HttpPost]
public IActionResult FinalizarCompra(int idUsuario)
{
    var carrito = ObtenerCarrito();  // Obtener el carrito actual desde la sesión

    if (!carrito.Items.Any())  // Verificar si el carrito está vacío
    {
        ModelState.AddModelError("", "El carrito está vacío.");
        return RedirectToAction("SeleccionarProductos");  // Si el carrito está vacío, redirigir al usuario
    }

    // Guardar solo los detalles del pedido en la tabla Detallespedido
    foreach (var item in carrito.Items)
    {
        var detallePedido = new Detallespedido
        {
            id_producto = item.id_producto,  // Asignar el id del producto
            cantidad = item.cantidad,  // Asignar la cantidad del producto
            total = item.total,  // Asignar el total (precio * cantidad)
            
        };

        // Guardar el detalle del pedido en la base de datos
        _context.Detallespedido.Add(detallePedido);
    }

    // Guardar los cambios en la base de datos
    _context.SaveChanges();

    // Vaciar el carrito después de finalizar la compra
    carrito.Vaciar();
    GuardarCarrito(carrito);

    // Redirigir al usuario a la vista de "Pedido Exitoso"
    return RedirectToAction("PedidoExitoso");
}


    // Métodos auxiliares para manejar el carrito en sesión
    private Carrito ObtenerCarrito()
    {
        var carrito = HttpContext.Session.GetObject<Carrito>("Carrito") ?? new Carrito();
        return carrito;
    }

    private void GuardarCarrito(Carrito carrito)
    {
        HttpContext.Session.SetObject("Carrito", carrito);
    }
}
