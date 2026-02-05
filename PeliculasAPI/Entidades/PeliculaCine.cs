namespace PeliculasAPI.Entidades
{
    public class PeliculaCine
    {
        public int CineId { get; set; }
        public int PeliculaId { get; set; }
        public Cine Cine { get; set; } = null!; // Propiedad de navegacion
        public Pelicula Pelicula { get; set; } = null!; // Propiedad de navegacion
    }
}
