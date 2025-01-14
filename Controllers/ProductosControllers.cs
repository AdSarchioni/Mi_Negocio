
using Microsoft.AspNetCore.Mvc;
using Mi_Negocio.Models;
using Mi_Negocio.Repositorios;

namespace Mi_Negocio.Controllers
{
    public class ProductosController : Controller
    {
        private readonly RepositorioProductos _repositorioProductos;
        private readonly IWebHostEnvironment _environment;
        // Constructor que inyecta el repositorio
        public ProductosController(RepositorioProductos repositorioProductos, IWebHostEnvironment environment)
        {
            _repositorioProductos = repositorioProductos;
            _environment = environment;
        }

        // Acción para listar productos
        public async Task<IActionResult> Index()
        {
            // Obtener todos los productos del repositorio
            var productos = await _repositorioProductos.ObtenerProductos();
            return View(productos);  // Pasamos la lista de productos a la vista
        }

        // Acción para crear un producto
        public IActionResult Crear()
        {
            return View();
        }

     // Acción para procesar el formulario y crear el producto

[HttpPost]
public async Task<IActionResult> CrearAsync(Producto producto)
{
    if (ModelState.IsValid)
    {
        try
        {
            string wwwPath = _environment.WebRootPath;
            string path = Path.Combine(wwwPath, "imagenes");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (producto.ImagenArchivo != null)
            {
                string fileName = "producto_" + Guid.NewGuid().ToString() + Path.GetExtension(producto.ImagenArchivo.FileName);
                string fullPath = Path.Combine(path, fileName);
                producto.imagen = $"/imagenes/{fileName}";

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await producto.ImagenArchivo.CopyToAsync(stream);
                }
            }
            else
            {
                producto.imagen = "/imagenes/default.jpg";
            }

            _repositorioProductos.InsertarProducto(producto);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Ocurrió un error: " + ex.Message);
        }
    }

    return View(producto);
}




      [HttpGet]
public IActionResult Editar(int id)
{
    var producto = _repositorioProductos.ObtenerProductoPorId(id);
    if (producto == null)
    {
        return NotFound();
    }
    return View(producto);
}
[HttpPost]
public async Task<IActionResult> EditarAsync(Producto producto)
{
    if (ModelState.IsValid)
    {
        try
        {
            string wwwPath = _environment.WebRootPath;
            string path = Path.Combine(wwwPath, "imagenes");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Procesar la nueva imagen si se carga una
            if (producto.ImagenArchivo != null)
            {
                // Generar un nuevo nombre para la imagen
                string fileName = "producto_" + Guid.NewGuid().ToString() + Path.GetExtension(producto.ImagenArchivo.FileName);
                string fullPath = Path.Combine(path, fileName);
                string nuevaRutaImagen = $"/imagenes/{fileName}";

                // Guardar la nueva imagen
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await producto.ImagenArchivo.CopyToAsync(stream);
                }

                // Eliminar la imagen anterior si no es la predeterminada
                if (!string.IsNullOrEmpty(producto.imagen) && producto.imagen != "/imagenes/default.jpg")
                {
                    string rutaImagenAnterior = Path.Combine(wwwPath, producto.imagen.TrimStart('/'));
                    if (System.IO.File.Exists(rutaImagenAnterior))
                    {
                        System.IO.File.Delete(rutaImagenAnterior);
                        Console.WriteLine($"Imagen anterior eliminada: {rutaImagenAnterior}");
                    }
                }

                // Actualizar la propiedad 'imagen' con la nueva ruta
                producto.imagen = nuevaRutaImagen;
            }
            else
            {
                // Si no se carga una nueva imagen, conserva la actual
                Console.WriteLine("No se cargó una nueva imagen. Se conserva la imagen existente.");
            }

            // Actualizar el producto en la base de datos
            var resultado = _repositorioProductos.EditarProducto(producto);

            if (resultado) // Verificar si la operación fue exitosa
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "No se pudo actualizar el producto en la base de datos.");
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Ocurrió un error: " + ex.Message);
        }
    }

    // Si llega aquí, algo falló
    return View(producto);
}

[HttpPost]
public IActionResult Alta(int id)
{
    _repositorioProductos.DarAlta(id);
    return RedirectToAction(nameof(Index));
}




[HttpPost]
public IActionResult Eliminar(int id)
{
    try
    {
        // Obtener el producto por ID
        var producto = _repositorioProductos.ObtenerProductoPorId(id);
        if (producto != null)
        {
            // Cambiar el estado a 0 (inactivo)
            producto.estado = 0;
            var resultado = _repositorioProductos.EditarProducto(producto);

            if (resultado)
            {
                TempData["SuccessMessage"] = "El producto se deshabilitó correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo deshabilitar el producto.";
            }
        }
        else
        {
            TempData["ErrorMessage"] = "No se encontró el producto.";
        }
    }
    catch (Exception ex)
    {
        TempData["ErrorMessage"] = $"Ocurrió un error: {ex.Message}";
    }

    return RedirectToAction("Index");
}





    }
}
