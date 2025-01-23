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
            var direcciones = await _context.Direcciones.Where(d => d.estado == 1).ToListAsync();
            return View(direcciones);
        }


        // GET: Direcciones/Details/5
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

            return View(direccion);
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
            if (ModelState.IsValid)
            {
                direccion.estado = 1;
                _context.Add(direccion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usuarios"] = new SelectList(_context.Usuarios, "usuarioId", "nombre","apellido");
            return View(direccion);
        }
         // GET: Direcciones/Delete/5
        public async Task<IActionResult> Editar(int? id)
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
                return RedirectToAction(nameof(Index));
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