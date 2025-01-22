using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mi_Negocio.Models;
public class Pedido
{
    [Key]
    public int pedidoId { get; set; }
    public int usuarioId { get; set; }
    public Usuario Usuario { get; set; } // Propiedad de navegación
    public DateTime fechaPedido { get; set; }
    public double total { get; set; }
    public int estado { get; set; }
        // Propiedad de navegación
    public ICollection<Detallespedido> Detallespedido { get; set; } // Nombre correcto
}