using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAppPeliculas.Models;

namespace WebAppPeliculas.Models
{
    public class PeliculaActor
    {
        [Key]
        public int IdPeliculaActor { get; set; }

        [Required(ErrorMessage = "La PELICULA es obligatoria")]
        [Display(Name = "Pelicula")]
        public int IdPelicula { get; set; }
        [ForeignKey(nameof(IdPelicula))]
        public Pelicula? Pelicula { get; set; }

        [Required(ErrorMessage = "El ACTOR es obligatorio")]
        [Display(Name = "Actor")]
        public int IdActor { get; set; }
        [ForeignKey(nameof(IdActor))]
        public Actor? Actor { get; set; }
    }
}
