

namespace Mi_Negocio.Models;
public class Pedido
{
    public int id_pedido { get; set; }
    public DateTime Fecha { get; set; }
    public int id_usuario { get; set; }
    public Usuario usuario { get; set; }
    public int id_direccion { get; set; }
    public Direccion direccion { get; set; }
    public ICollection<Detallespedido> detalles { get; set; }
    public decimal Total { get; set; }
    public int Estado { get; set; }
}