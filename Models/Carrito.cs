namespace Mi_Negocio.Models;

public class Carrito
{
    public List<Detallespedido> Items { get; set; } = new List<Detallespedido>();

    public double Total => Items.Sum(item => item.total);

    public void Vaciar()
    {
        Items.Clear();
    }

    public void AgregarProducto(Producto producto, int cantidad)
    {
        var detalleExistente = Items.FirstOrDefault(item => item.productoId == producto.id_producto);
        if (detalleExistente != null)
        {
            detalleExistente.cantidad += cantidad;
            detalleExistente.total = detalleExistente.cantidad * producto.precio;
        }
        else
        {
            Items.Add(new Detallespedido
            {
            
                productoId = producto.id_producto,
                cantidad = cantidad,
                total = producto.precio * cantidad
            });
        }
    }
}
