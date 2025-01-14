
using Mi_Negocio.Data;
using Mi_Negocio.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mi_Negocio.Repositorios
{
    public class RepositorioProductos
    {
        private readonly DataContext _context;

        // Constructor que recibe el contexto de la base de datos
        public RepositorioProductos(DataContext context)
        {
            _context = context;
        }

        // Método para obtener todos los productos
        public async Task<List<Producto>> ObtenerProductos()
        {
            return await _context.Productos.ToListAsync();  // Consulta todos los productos en la tabla Productos
        }

           // Método para obtener un producto por ID
        public async Task<Producto> GetProductoByIdAsync(int id)
        {
            return await _context.Productos.FirstOrDefaultAsync(p => p.id_producto == id);
        }

        // Método para agregar un nuevo producto
        public async Task AddProductoAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
        }

     // Método para insertar un nuevo producto
        public void InsertarProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            _context.SaveChanges(); // Guarda los cambios en la base de datos
        }

public bool EditarProducto(Producto producto)
{
    try
    {
        _context.Productos.Update(producto);
        _context.SaveChanges();
        return true;
    }
    catch
    {
        return false;
    }
}


    // Eliminar un producto
    public void EliminarProducto(int id)
    {
        var producto = _context.Productos.Find(id);
        if (producto != null)
        {
            _context.Productos.Remove(producto);
            _context.SaveChanges();
        }
    }

    // Obtener un producto por ID (para cargar en el formulario de edición)
    public Producto? ObtenerProductoPorId(int id)
    {
        return _context.Productos.FirstOrDefault(p => p.id_producto == id);
    }
  public void DarAlta(int id)
    {
        var producto = _context.Productos.FirstOrDefault(p => p.id_producto == id);
        if (producto != null && producto.estado == 0)
        {
            producto.estado = 1; // Cambiar el estado a activo
            _context.SaveChanges();
        }

    }
    }
    }
