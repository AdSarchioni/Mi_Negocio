using Mi_Negocio.Data;
using Mi_Negocio.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mi_Negocio.Repositorios
{
    public class RepositorioUsuarios
    {
        private readonly DataContext _context;

        public RepositorioUsuarios(DataContext context)
        {
            _context = context;
        }

       

        // Obtener todos los usuarios
        public async Task<List<Usuario>> ObtenerUsuariosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // Obtener usuario por ID
        public async Task<Usuario> ObtenerUsuarioPorIdAsync(int id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.id_usuario == id);
        }

        // Agregar un nuevo usuario
        public async Task AgregarUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        // Insertar un usuario (sincrónico)
        public void InsertarUsuario(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        // Editar un usuario
        public bool EditarUsuario(Usuario usuario)
        {
            try
            {
                _context.Usuarios.Update(usuario);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Eliminar un usuario por ID
        public void EliminarUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
            }
        }

        // Cambiar estado del usuario a activo
        public void DarAlta(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.id_usuario == id);
            if (usuario != null && usuario.estado_usuario == 0)
            {
                usuario.estado_usuario = 1; // Cambiar a activo
                _context.SaveChanges();
            }
        }

        // Manejar avatar del usuario (para cargar archivo)
        public async Task ActualizarAvatarAsync(int id, string avatarPath)
        {
            var usuario = await ObtenerUsuarioPorIdAsync(id);
            if (usuario != null)
            {
                usuario.avatar = avatarPath;
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
            }
        }

public async Task<Usuario?> ObtenerUsuarioLoginAsync(string email, string password)
{

    {
        // Buscar el usuario con el email y contraseña proporcionados
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.email == email && u.password == password);

        return usuario; // Retornar el usuario encontrado o null si no existe

    }
    
}




public async Task<int> EsIgualPasswordAsync(int id, string password)
{
    var res= -1;
    try
    {
        // Verificar si existe un usuario con el ID y contraseña proporcionados
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.id_usuario == id && u.password == password);

        // Si el usuario existe, retornar 1; de lo contrario, -1
        return res = 1 ;
    }
    catch
    {
        // En caso de error, retornar -1
        return res;
    }
}






public async Task<int> UpdateClaveAsync(int id, string password)
{
    try
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.id_usuario == id);

        if (usuario != null)
        {
            usuario.password = password;
            await _context.SaveChangesAsync();
            return 1; // Éxito
        }

        return -1; // Usuario no encontrado
    }
    catch
    {
        return -1; // Error
    }
}



        
    }
}
