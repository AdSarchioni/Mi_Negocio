
using System.ComponentModel.DataAnnotations;

namespace Mi_Negocio.Models;
public class Direccion
{
    [Key]
    public int direccionId { get; set; }
    public string calleDireccion { get; set; }
    public string ciudad { get; set; }
    public string provincia { get; set; }
    public string codigoPostal { get; set; }
    public int usuarioId { get; set; }
    public int estado { get; set; }
    public Usuario Usuario { get; set; }  // Propiedad de navegación
}