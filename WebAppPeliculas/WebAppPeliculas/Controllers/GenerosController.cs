using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpreadsheetLight;
using WebAppPeliculas.Models;

namespace WebAppPeliculas.Controllers
{
    public class GenerosController : Controller
    {
        private readonly AppDBcontext _context;
        private readonly IWebHostEnvironment _env;
        public GenerosController(AppDBcontext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // IMPORT
        public async Task<IActionResult> ImportarCsv()
        {
            var archivos = HttpContext.Request.Form.Files;
            if (archivos != null && archivos.Count > 0)
            {
                var archivo = archivos[0];
                if (archivo.Length > 0)
                {
                    var pathDestino = Path.Combine(_env.WebRootPath, "importaciones");
                    var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(archivo.FileName);
                    var rutaDestino = Path.Combine(pathDestino, archivoDestino);

                    // Asegúrate de que la carpeta de destino exista
                    if (!Directory.Exists(pathDestino))
                    {
                        Directory.CreateDirectory(pathDestino);
                    }

                    using (var filestream = new FileStream(rutaDestino, FileMode.Create))
                    {
                        archivo.CopyTo(filestream);
                    }

                    try
                    {
                        using (var file = new FileStream(rutaDestino, FileMode.Open))
                        {
                            List<string> renglones = new List<string>();
                            List<Genero> GenerosArch = new List<Genero>();

                            using (StreamReader fileContent = new StreamReader(file, System.Text.Encoding.UTF8))
                            {
                                while (!fileContent.EndOfStream)
                                {
                                    renglones.Add(fileContent.ReadLine());
                                }
                            }
                            if (renglones.Count > 0)
                            {
                                foreach (var renglon in renglones)
                                {
                                    string[] data = renglon.Split(';');
                                    if (data.Length == 1)
                                    {
                                        Genero genero = new Genero
                                        {
                                            Descripcion = data[0].Trim(),
                                        };
                                        GenerosArch.Add(genero);
                                    }
                                }

                                if (GenerosArch.Count > 0)
                                {
                                    _context.AddRange(GenerosArch);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Manejo de errores al leer el archivo o guardar en la base de datos
                        ModelState.AddModelError("", $"Error al procesar el archivo: {ex.Message}");

                        // Crea un modelo vacío o uno con datos relevantes para la vista "Error"
                        var errorModel = new ErrorViewModel
                        {
                            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                        };
                        return View("Error", errorModel);
                    }
                }
            }
            return RedirectToAction("Index", "Generos");
        }
        public async Task<IActionResult> ImportarExcel()
        {
            var archivos = HttpContext.Request.Form.Files;
            if (archivos != null && archivos.Count > 0)
            {
                var archivoExcel = archivos[0];
                if (archivoExcel.Length > 0)
                {
                    var rutaDestino = Path.Combine(_env.WebRootPath, "importaciones");
                    var extArch = Path.GetExtension(archivoExcel.FileName);
                    if (extArch == ".xlsx" || extArch == ".xls")
                    {
                        var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + extArch;
                        var rutaCompleta = Path.Combine(rutaDestino, archivoDestino);

                        using (var filestream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            archivoExcel.CopyTo(filestream);
                        }

                        using SLDocument archXls = new SLDocument(rutaCompleta);
                        if (archXls != null)
                        {
                            List<Genero> ListaGeneros = new List<Genero>();

                            int fila = 1;
                            while (!string.IsNullOrEmpty(archXls.GetCellValueAsString(fila, 1))) 
                            {
                                try
                                {
                                    Genero genero = new Genero
                                    {
                                        Descripcion = archXls.GetCellValueAsString(fila, 1), // Descripcion de generos
                                    };

                                    ListaGeneros.Add(genero);
                                }
                                catch (Exception ex)
                                {
                                    ViewData["Error"] = $"Error al procesar la fila {fila}: {ex.Message}";
                                    return View("Error");
                                }
                                fila++;
                            }
                            if (ListaGeneros.Count > 0)
                            {
                                _context.AddRange(ListaGeneros);
                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                ViewData["Error"] = "No se encontraron datos válidos en el archivo Excel.";
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Generos");
        }

        // GET: Generos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Generos.ToListAsync());
        }

        // GET: Generos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = await _context.Generos
                .FirstOrDefaultAsync(m => m.IdGenero == id);
            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }

        // GET: Generos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Generos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdGenero,Descripcion")] Genero genero)
        {
            if (ModelState.IsValid)
            {
                _context.Add(genero);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(genero);
        }

        // GET: Generos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = await _context.Generos.FindAsync(id);
            if (genero == null)
            {
                return NotFound();
            }
            return View(genero);
        }

        // POST: Generos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdGenero,Descripcion")] Genero genero)
        {
            if (id != genero.IdGenero)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genero);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneroExists(genero.IdGenero))
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
            return View(genero);
        }

        // GET: Generos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = await _context.Generos
                .FirstOrDefaultAsync(m => m.IdGenero == id);
            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }

        // POST: Generos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genero = await _context.Generos.FindAsync(id);
            if (genero != null)
            {
                _context.Generos.Remove(genero);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GeneroExists(int id)
        {
            return _context.Generos.Any(e => e.IdGenero == id);
        }
    }
}
