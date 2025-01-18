
namespace Mi_Negocio.Models;

public class Detallespedido
{
    public int id_detallePedido { get; set; }
    public int id_pedido { get; set; }
    public DateTime fechaPedido { get; set; }
    public int id_producto { get; set; }
    
    public int cantidad { get; set; }
    public double total { get; set; }
    public int estado { get; set; }
}