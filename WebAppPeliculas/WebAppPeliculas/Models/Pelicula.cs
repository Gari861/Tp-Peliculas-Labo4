using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebAppPeliculas.Models;

namespace WebAppPeliculas.Models
{
    public class Pelicula
    {
        [Key]
        public int IdPelicula { get; set; }

        [Required(ErrorMessage = "El Título es obligatorio")]
        [Display(Name = "Título")]
        public string? Titulo { get; set; }

        [Required(ErrorMessage = "La FECHA DE ESTRENO es obligatoria")]
        [Display(Name = "Fecha de Estreno")]
        public DateOnly FechaEstreno { get; set; }

        [Display(Name = "Portada")]
        public string? Portada { get; set; }

        [Required(ErrorMessage = "El TRAILER es obligatorio")]
        [Display(Name = "Trailer")]
        public string? Trailer { get; set; }

        [Required(ErrorMessage = "El RESUMEN es obligatorio")]
        [Display(Name = "Resumen")]
        public string? Resumen { get; set; }

        //Relación UNO A MUCHOS
        [Display(Name = "Genero")]
        public int IdGenero { get; set; }
        [ForeignKey(nameof(IdGenero))]
        public Genero? Genero { get; set; }

        //Relación MUCHOS A MUCHOS
        [Display(Name = "Actores")]
        public List<PeliculaActor>? PeliculasActores { get; set; }
    }
}
