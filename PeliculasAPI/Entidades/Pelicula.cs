using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Entidades
{
    public class Pelicula : IId
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(300, ErrorMessage = "El campo {0} debe contener {1} caracteres o menos")]
        [PrimeraLetraMayuscula]
        public required string Titulo { get; set; }
        public string? Trailer { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        [Unicode(false)] // Va a ser una URL y no se necesitan todos los caracteres Unicode (varchar y no nvarchar)
        public string? Poster { get; set; }
        public List<PeliculaGenero> PeliculaGeneros { get; set; } = new List<PeliculaGenero>(); // Propiedad de navegacion a la tabla intermedia PeliculaGenero
        public List<PeliculaCine> PeliculaCines { get; set; } = new List<PeliculaCine>(); // Propiedad de navegacion a la tabla intermedia PeliculaCine
        public List<PeliculaActor> PeliculaActores { get; set; } = new List<PeliculaActor>(); // Propiedad de navegacion a la tabla intermedia PeliculaActor
    }
}
