
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mi_Negocio.Models;


    public enum enRoles
{
    Administrador = 1,
    Cliente = 2,
}

    public class Usuario
   {
   [Key]
    public int usuarioId { get; set; }

   [Required(ErrorMessage = "El apellido es obligatorio.")]
    public string? apellido { get; set; }

  [Required(ErrorMessage = "El Nombre es obligatorio.")]
    public string? nombre { get; set; }

  [Required(ErrorMessage = "El DNI es obligatorio.")]
  
    public int dni { get; set; }

   [Required(ErrorMessage = "El teléfono es obligatorio.")]
    public string? telefono { get; set; }
    [Required(ErrorMessage = "El email es obligatorio.")]
    
    public string? email { get; set; }
   
    public string? password { get; set; }

    public string? avatar { get; set; }

    [NotMapped]
    public IFormFile? avatarFile { get; set; }
    [Required(ErrorMessage = "El estado es obligatorio.")]
    public int estado_usuario { get; set; }
    public int rol { get; set; }
    [NotMapped]//Para EF
    public string rolnombre => rol > 0 ? ((enRoles)rol).ToString() : "";
   

    public static IDictionary<int, string> ObtenerRoles()
    {
        SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
        Type tipoEnumRol = typeof(enRoles);
        foreach (var valor in Enum.GetValues(tipoEnumRol))
        {
            roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor));
        }
        return roles;
    }
    public override string ToString()
    {
        return $"{apellido},{nombre}-({usuarioId})";
    }

}