using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Mi_Negocio.Models
{
    public class Producto
    {
    [Key]   
    public int id_producto { get; set; }
    public string? nombre { get; set; }
    public string? descripcion { get; set; }
    public double precio { get; set; }
    public double stock { get; set; }
    public int id_categoria { get; set; }
    public string? imagen { get; set; }
    public int estado { get; set; }

    [NotMapped]
    public IFormFile? ImagenArchivo { get; set; }
    }
}