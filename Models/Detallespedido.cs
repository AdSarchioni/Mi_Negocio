

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mi_Negocio.Models;
public class Detallespedido
{
    [Key]
    public int detalleId { get; set; }
    public string nombre { get; set; }
    public string imagen { get; set; }
    public DateTime fechaPedido { get; set; }
    
    public int productoId { get; set; }
    public int pedidoId { get; set; }
    // Propiedad de navegación
    public Producto Producto { get; set; } 

    public double cantidad { get; set; }
    public double precioUnitario { get; set; }
    public double total { get; set; }
    public int estado { get; set; }
}