
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
    public int Id_Usuario { get; set; }

 [Required(ErrorMessage = "El apellido es obligatorio.")]
    public string? Apellido { get; set; }

[Required(ErrorMessage = "El Nombre es obligatorio.")]
    public string? Nombre { get; set; }

  [Required(ErrorMessage = "El DNI es obligatorio.")]
    [RegularExpression(@"^\d{1,10}$", ErrorMessage = "El DNI debe contener solo números y un máximo de 11 dígitos.")]
    public int Dni { get; set; }

   [Required(ErrorMessage = "El teléfono es obligatorio.")]
    public string? Telefono { get; set; }
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "El Password es obligatorio.")]
    public string? Password { get; set; }

    public string? Avatar { get; set; }

    [NotMapped]
    public IFormFile? AvatarFile { get; set; }
    [Required(ErrorMessage = "El estado es obligatorio.")]
    public int Estado_Usuario { get; set; }
    public int Rol { get; set; }
    [NotMapped]//Para EF
    public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

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
        return $"{Apellido},{Nombre}-({Id_Usuario})";
    }
}