using System.ComponentModel.DataAnnotations;
using WebAppPeliculas.Models;

namespace WebAppPeliculas.Models
{
    public class Actor
    {
        [Key]
        public int IdAutor { get; set; }

        [Required(ErrorMessage = "El NOMBRE es obligatorio")]
        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [Required(ErrorMessage = "La FECHA DE NACIMIENTO es obligatoria")]
        [Display(Name = "Fecha de Nacimiento")]
        public DateOnly FechaNacimiento { get; set; }

        [Display(Name = "Foto")]
        public string? Foto { get; set; }

        //Relación MUCHOS A MUCHOS
        public List<PeliculaActor>? PeliculasActores { get; set; }
    }
}
