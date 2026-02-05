using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Entidades
{
    public class PeliculaActor
    {
        public int ActorId { get; set; }
        public int PeliculaId { get; set; }
        [StringLength(300, ErrorMessage = "El campo {0} debe contener {1} caracteres o menos")]
        public required string Personaje { get; set; }
        public int Orden { get; set; }
        public Actor Actor { get; set; } = null!; // Propiedad de navegacion
        public Pelicula Pelicula { get; set; } = null!; // Propiedad de navegacion
    }
}
