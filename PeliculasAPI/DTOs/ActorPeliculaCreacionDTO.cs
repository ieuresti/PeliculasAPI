namespace PeliculasAPI.DTOs
{
    // DTO para relacionar actores con peliculas al momento de crear o editar una pelicula
    public class ActorPeliculaCreacionDTO
    {
        public int Id { get; set; }
        public required string Personaje { get; set; }
    }
}
