using System.ComponentModel.DataAnnotations;
using WebAppPeliculas.Models;

namespace WebAppPeliculas.Models
{
    public class Genero
    {
        [Key]
        public int IdGenero { get; set; }

        [Required(ErrorMessage = "La DESCRIPCION es obligatorio")]
        [Display(Name = "Descripcion")]
        public string? Descripcion { get; set; }

        //Relación UNO A MUCHOS
        public List<Pelicula>? Peliculas { get; set; }
    }
}
