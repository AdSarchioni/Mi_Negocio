
using System.ComponentModel.DataAnnotations;
namespace Mi_Negocio.Models;

public class DireccionConUsuario
{
    public Direccion Direccion { get; set; }
    public string NombreUsuario { get; set; }
    public string ApellidoUsuario { get; set; }
}
