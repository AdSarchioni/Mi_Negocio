using Mi_Negocio.Data;
using Mi_Negocio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

public class CarritoController : Controller
{
    private readonly DataContext _context;

    public CarritoController(DataContext context)
    {
        _context = context;
    }

    // Acción para mostrar todos los productos disponibles
public IActionResult SeleccionarProductos(int? direccionId)
{
    var productos = _context.Productos.ToList(); // Lista de productos

    // Obtener las direcciones del usuario actual
    var idUsuario = User.FindFirstValue(ClaimTypes.PrimarySid);
    var direcciones = _context.Direcciones
        .Where(d => d.usuarioId == int.Parse(idUsuario))
        .ToList();

    // Si se pasó un direccionId, asignarlo al ViewBag
    ViewBag.SelectedDireccionId = direccionId ?? direcciones.FirstOrDefault()?.direccionId;

    ViewBag.Direcciones = direcciones; // Pasar las direcciones a la vista

    return View(productos);
}


    // Acción para agregar un producto al carrito
    [HttpPost]
    public IActionResult AgregarProductoAlCarrito(int productoId, int cantidad)
    {
        var producto = _context.Productos.Find(productoId);
        if (producto == null)
        {
            return NotFound("Producto no encontrado.");
        }

        // Obtener el carrito de la sesión
        var carrito = ObtenerCarrito();

        // Si el producto ya está en el carrito, aumentamos la cantidad
        var detalleExistente = carrito.Items.FirstOrDefault(item => item.productoId == productoId);
        if (detalleExistente != null)
        {
            detalleExistente.cantidad += cantidad;
        }
        else
        {
            // Si no está en el carrito, lo agregamos
            carrito.Items.Add(new Detallespedido
            {
                productoId = productoId,
                nombre = producto.nombre,
                imagen = producto.imagen,
                cantidad = cantidad,
                total = producto.precio * cantidad
            });
        }

        // Guardamos el carrito en la sesión
        GuardarCarrito(carrito);

// Log para inspeccionar los datos
Console.WriteLine("Contenido del carrito:");
foreach (var item in carrito.Items)
{
    Console.WriteLine($"Producto ID: {item.productoId},Nombre: {item.nombre}, Cantidad: {item.cantidad}, Total: {item.total}");
}



        return RedirectToAction("SeleccionarProductos");
    }

    // Acción para finalizar la compra y guardar en Detallespedido
[HttpPost]
public IActionResult FinalizarCompra(int direccionId)
{


    
    // Obtener el id_usuario desde las claims
    var idUsuarioClaim = User.FindFirst(ClaimTypes.PrimarySid); // Usar PrimarySid como definido en las claims
    if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuario))
    {
        // Si no se encuentra el claim o no es válido, redirigir con un mensaje de error
        ModelState.AddModelError("", "No se pudo identificar al usuario.");
        return RedirectToAction("SeleccionarProductos");
    }

    var carrito = ObtenerCarrito(); // Obtener el carrito actual desde la sesión

    if (!carrito.Items.Any()) // Verificar si el carrito está vacío
    {
        ModelState.AddModelError("", "El carrito está vacío.");
        return RedirectToAction("SeleccionarProductos"); // Si el carrito está vacío, redirigir al usuario
    }

    // Validar que la dirección seleccionada pertenece al usuario actual
    var direccion = _context.Direcciones.FirstOrDefault(d => d.direccionId == direccionId && d.usuarioId == idUsuario);
    if (direccion == null)
    {
        ModelState.AddModelError("", "La dirección seleccionada no es válida.");
        return RedirectToAction("SeleccionarProductos");
    }

    // Calcular el total del pedido sumando los totales de los productos en el carrito
    decimal totalPedido = (decimal)carrito.Items.Sum(item => item.total);

    // Crear el registro del pedido en la tabla Pedido
    var pedido = new Pedido
    {
        usuarioId = idUsuario,
        fechaPedido = DateTime.Now,
        total = (double)totalPedido,
        estado = 1, // Estado inicial (ejemplo: 1 = Pendiente)
        direccionId = direccionId // Guardar la dirección seleccionada
    };

    _context.Pedidos.Add(pedido);
    _context.SaveChanges(); // Guardar el pedido para generar el PedidoId

    // Guardar los detalles del pedido en la tabla DetallePedido
    foreach (var item in carrito.Items)
    {
        var detallePedido = new Detallespedido
        {
            pedidoId = pedido.pedidoId, // Relacionar con el PedidoId recién creado
            productoId = item.productoId,
            nombre = item.nombre,
            cantidad = item.cantidad,
            precioUnitario = item.precioUnitario, // Precio por unidad en el momento de la compra
            total = item.total // Precio total para este producto (precioUnitario * cantidad)
        };

        _context.Detallespedido.Add(detallePedido);
    }

    // Guardar los detalles del pedido en la base de datos
    _context.SaveChanges();

    // Vaciar el carrito después de finalizar la compra
    carrito.Vaciar();
    GuardarCarrito(carrito);

    // Redirigir al usuario a la vista de "Pedido Exitoso"
    return RedirectToAction("PedidoExitoso", new { pedidoId = pedido.pedidoId });
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
public IActionResult PedidoExitoso(int pedidoId)
{
    var pedido = _context.Pedidos
        .Include(p => p.Detallespedido) // Cargar los detalles del pedido
        .ThenInclude(dp => dp.Producto) // Cargar la información del producto
        .FirstOrDefault(p => p.pedidoId == pedidoId);

    if (pedido == null)
    {
        return NotFound("El pedido no existe.");
    }

    // Obtener la dirección asociada al pedido
    var direccion = _context.Direcciones
        .FirstOrDefault(d => d.direccionId == pedido.direccionId);

    // Pasar la dirección a través del ViewBag
    ViewBag.Direccion = direccion;

    return View(pedido);
}


public IActionResult VerCarrito()
{
    // Obtener el carrito desde la sesión
    var carrito = ObtenerCarrito();

    // Convertir el objeto Carrito en CarritoViewModel
    var model = new CarritoViewModel
    {
        Items = carrito.Items.Select(item => new DetalleCarrito
        {
            productoId = item.productoId, 
            nombre = item.nombre,
            imagen = item.imagen,
            cantidad = item.cantidad,
            precioUnitario = item.precioUnitario,
            total = item.total
        }).ToList()
    };

    return View(model); // Pasar el CarritoViewModel a la vista
}


[HttpPost]
public IActionResult EliminarDelCarrito(int idProducto)
{
    // Obtener el carrito desde la sesión
    var carrito = ObtenerCarrito();

    // Buscar el producto en el carrito
    var item = carrito.Items.FirstOrDefault(i => i.productoId == idProducto);
    if (item != null)
    {
        carrito.Items.Remove(item); // Eliminar el producto
        GuardarCarrito(carrito);    // Guardar los cambios
    }

    return RedirectToAction("VerCarrito"); // Redirigir al carrito actualizado
}


}
