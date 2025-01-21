

using System.ComponentModel.DataAnnotations;

namespace Mi_Negocio.Models;
public class Detallespedido
{
    [Key]
    public int id_detallePedido { get; set; }
   
    public DateTime fechaPedido { get; set; }
    public int id_producto { get; set; }

    // Propiedad de navegación
    public Producto Producto { get; set; } 

    public int cantidad { get; set; }
    public double total { get; set; }
    public int estado { get; set; }
}