using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppPeliculas.Models;

namespace WebAppPeliculas.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly AppDBcontext _context;
        private readonly IWebHostEnvironment _env;
        public PeliculasController(AppDBcontext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Peliculas
        public async Task<IActionResult> Index()
        {
            var appDBcontext = _context.Peliculas.Include(p => p.Genero);
            return View(await appDBcontext.ToListAsync());
        }

        // GET: Peliculas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Peliculas
                .Include(p => p.Genero)
                .FirstOrDefaultAsync(m => m.IdPelicula == id);
            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        // GET: Peliculas/Create
        public IActionResult Create()
        {
            ViewData["IdGenero"] = new SelectList(_context.Generos, "IdGenero", "Descripcion");
            return View();
        }

        // POST: Peliculas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPelicula,Titulo,FechaEstreno,Portada,Trailer,Resumen,IdGenero")] Pelicula pelicula)
        {
            if (ModelState.IsValid)
            {
                // Manejo del archivo de foto
                var archivos = HttpContext.Request.Form.Files;
                if (archivos != null && archivos.Count > 0)
                {
                    var archivoFoto = archivos[0];
                    if (archivoFoto.Length > 0)
                    {
                        var rutaDestino = Path.Combine(_env.WebRootPath, "fotografias");
                        var extArch = Path.GetExtension(archivoFoto.FileName);
                        // Generar un nombre único para el archivo
                        var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + extArch;

                        // Guardar el archivo en memoria
                        using (var filestream = new FileStream(Path.Combine(rutaDestino, archivoDestino), FileMode.Create))
                        {
                            archivoFoto.CopyTo(filestream);
                            pelicula.Portada = archivoDestino;
                        }
                    }
                }
                _context.Add(pelicula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdGenero"] = new SelectList(_context.Generos, "IdGenero", "Descripcion", pelicula.IdGenero);
            return View(pelicula);
        }

        // GET: Peliculas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula == null)
            {
                return NotFound();
            }
            ViewData["IdGenero"] = new SelectList(_context.Generos, "IdGenero", "Descripcion", pelicula.IdGenero);
            return View(pelicula);
        }

        // POST: Peliculas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPelicula,Titulo,FechaEstreno,Portada,Trailer,Resumen,IdGenero")] Pelicula pelicula)
        {
            if (id != pelicula.IdPelicula)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Manejo de archivos (fotos)
                    var archivos = HttpContext.Request.Form.Files;
                    if (archivos != null && archivos.Count > 0)
                    {
                        var archivoFoto = archivos[0];
                        if (archivoFoto.Length > 0)
                        {
                            var rutaDestino = Path.Combine(_env.WebRootPath, "fotografias");
                            var extArch = Path.GetExtension(archivoFoto.FileName);
                            var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + extArch;

                            // Crear el archivo en memoria
                            using (var filestream = new FileStream(Path.Combine(rutaDestino, archivoDestino), FileMode.Create))
                            {
                                archivoFoto.CopyTo(filestream);

                                // Si existe una foto anterior, se elimina
                                if (!string.IsNullOrEmpty(pelicula.Portada))
                                {
                                    string fotoAnterior = Path.Combine(rutaDestino, pelicula.Portada);
                                    if (System.IO.File.Exists(fotoAnterior))
                                    {
                                        System.IO.File.Delete(fotoAnterior);
                                    }
                                }

                                // Asignar la nueva foto
                                pelicula.Portada = archivoDestino;
                            }
                        }
                    }

                    // Actualizar libro en la base de datos
                    _context.Update(pelicula);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeliculaExists(pelicula.IdPelicula))
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
            ViewData["IdGenero"] = new SelectList(_context.Generos, "IdGenero", "Descripcion", pelicula.IdGenero);
            return View(pelicula);
        }

        // GET: Peliculas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Peliculas
                .Include(p => p.Genero)
                .FirstOrDefaultAsync(m => m.IdPelicula == id);
            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        // POST: Peliculas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula != null)
            {
                _context.Peliculas.Remove(pelicula);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PeliculaExists(int id)
        {
            return _context.Peliculas.Any(e => e.IdPelicula == id);
        }
    }
}
