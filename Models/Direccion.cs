
namespace Mi_Negocio.Models;
public class Direccion
{
    public int id_direccion { get; set; }
    public string calleDireccion { get; set; }
    public string ciudad { get; set; }
    public string provincia { get; set; }
    public string codigoPostal { get; set; }
    public int id_usuario { get; set; }

}