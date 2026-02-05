namespace PeliculasAPI.Entidades
{
    public class PeliculaGenero
    {
        public int GeneroId { get; set; }
        public int PeliculaId { get; set; }
        public Genero Genero { get; set; } = null!; // Propiedad de navegacion
        public Pelicula Pelicula { get; set; } = null!; // Propiedad de navegacion
    }
}
