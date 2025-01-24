using Microsoft.AspNetCore.Mvc;
using Mi_Negocio.Data;
using Mi_Negocio.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mi_Negocio.Controllers
{
    public class DireccionesController : Controller
    {
        private readonly DataContext _context;

        public DireccionesController(DataContext context)
        {
            _context = context;
        }
        // GET: Direcciones

     public async Task<IActionResult> Index()
{
    // Obtener todas las direcciones donde el estado sea 1
    var direcciones = await _context.Direcciones
        .Where(d => d.estado == 1)
        .ToListAsync();

    // Obtener los usuarios asociados a las direcciones
    var direccionConUsuarios = new List<DireccionConUsuario>();
    
    foreach (var direccion in direcciones)
    {
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.usuarioId == direccion.usuarioId);
        
        direccionConUsuarios.Add(new DireccionConUsuario
        {
            Direccion = direccion,
            NombreUsuario = usuario?.nombre,
            ApellidoUsuario = usuario?.apellido
        });
    }

    return View(direccionConUsuarios);
}


   public async Task<IActionResult> Detalles(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var direccion = await _context.Direcciones
        .FirstOrDefaultAsync(m => m.direccionId == id && m.estado == 1);

    if (direccion == null)
    {
        return NotFound();
    }

    // Obtener el usuario asociado a la dirección
    var usuario = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.usuarioId == direccion.usuarioId);

    var direccionConUsuario = new DireccionConUsuario
    {
        Direccion = direccion,
        NombreUsuario = usuario?.nombre,
        ApellidoUsuario = usuario?.apellido
    };

    return View(direccionConUsuario);
}
   
        // GET: Direcciones/Create
        public IActionResult Crear()
        {
            // Concatenar nombre y apellido para mostrar en el SelectList
            var usuarios = _context.Usuarios
                .Select(u => new
                {
                    usuarioId = u.usuarioId,
                    NombreCompleto = u.nombre + " " + u.apellido
                })
                .ToList();

            ViewData["Usuarios"] = new SelectList(usuarios, "usuarioId", "NombreCompleto");
            return View();
        }

        // POST: Direcciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("direccionId,calleDireccion,ciudad,provincia,codigoPostal,usuarioId")] Direccion direccion)
        {
            Console.WriteLine("UsuarioId: " + direccion.usuarioId);
            if (ModelState.IsValid)
            {
                direccion.estado = 1;
                _context.Add(direccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuarios"] = new SelectList(_context.Usuarios, "usuarioId", "nombre", "apellido");
            return View(direccion);
        }

// GET: Direcciones/Edit/5
public async Task<IActionResult> Editar(int id)
{
    // Obtener la dirección
    var direccion = await _context.Direcciones
        .FirstOrDefaultAsync(d => d.direccionId == id);

    if (direccion == null)
    {
        return NotFound();
    }

    // Obtener el usuario asociado a esta dirección
    var usuario = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.usuarioId == direccion.usuarioId);

    // Pasar el nombre y apellido del usuario al ViewBag
    if (usuario != null)
    {
        ViewBag.NombreUsuario = usuario.nombre;
        ViewBag.ApellidoUsuario = usuario.apellido;
    }

    return View(direccion);
}

// POST: Direcciones/Edit/5
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Editar(int id, [Bind("direccionId,calleDireccion,ciudad,provincia,codigoPostal,usuarioId,estado")] Direccion direccion)
{
    if (id != direccion.direccionId)
    {
        return NotFound();
    }

    // Cargar la dirección existente desde la base de datos
    var direccionExistente = await _context.Direcciones
        .AsNoTracking()
        .FirstOrDefaultAsync(d => d.direccionId == id);

    if (direccionExistente == null)
    {
        return NotFound();
    }

    // Mantener datos que no se enviaron en el formulario
    direccion.usuarioId = direccionExistente.usuarioId;

    if (ModelState.IsValid)
    {
        try
        {
            _context.Update(direccion);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DireccionExists(direccion.direccionId))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        // Después de la actualización, obtener y pasar los datos del usuario nuevamente
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.usuarioId == direccion.usuarioId);

        if (usuario != null)
        {
            ViewBag.NombreUsuario = usuario.nombre;
            ViewBag.ApellidoUsuario = usuario.apellido;
        }

        // Redirigir a la vista con la información del usuario
         return RedirectToAction(nameof(Index));
    }

    // Si el modelo no es válido, cargar los datos necesarios para la vista
    var usuarioInvalid = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.usuarioId == direccion.usuarioId);

    if (usuarioInvalid != null)
    {
        ViewBag.NombreUsuario = usuarioInvalid.nombre;
        ViewBag.ApellidoUsuario = usuarioInvalid.apellido;
    }

    return View(direccion);
}





        // POST: Direcciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var direccion = await _context.Direcciones.FindAsync(id);
            if (direccion != null)
            {
                direccion.estado = 0; // Marcar como inactiva en lugar de eliminar
                _context.Update(direccion);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DireccionExists(int id)
        {
            return _context.Direcciones.Any(e => e.direccionId == id && e.estado == 1);
        }
    }
}