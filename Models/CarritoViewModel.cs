
namespace Mi_Negocio.Models;
public class CarritoViewModel
{
    public List<DetalleCarrito> Items { get; set; } = new List<DetalleCarrito>();
}

public class DetalleCarrito
{
    public int productoId { get; set; }
    public string nombre { get; set; }   
    public string imagen { get; set; }       // Nombre del producto
    public double cantidad { get; set; }         // Cantidad seleccionada
    public double precioUnitario { get; set; } // Precio unitario
    public double total { get; set; }        // Precio total (cantidad * precio unitario)
}
