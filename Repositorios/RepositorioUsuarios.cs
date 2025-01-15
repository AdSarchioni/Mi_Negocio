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
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id_Usuario == id);
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
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id_Usuario == id);
            if (usuario != null && usuario.Estado_Usuario == 0)
            {
                usuario.Estado_Usuario = 1; // Cambiar a activo
                _context.SaveChanges();
            }
        }

        // Manejar avatar del usuario (para cargar archivo)
        public async Task ActualizarAvatarAsync(int id, string avatarPath)
        {
            var usuario = await ObtenerUsuarioPorIdAsync(id);
            if (usuario != null)
            {
                usuario.Avatar = avatarPath;
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();
            }
        }
    }
}
