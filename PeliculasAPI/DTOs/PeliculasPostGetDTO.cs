namespace PeliculasAPI.DTOs
{
    // DTO para obtener los datos necesarios para crear o editar una pelicula
    public class PeliculasPostGetDTO
    {
        public List<GeneroDTO> Generos { get; set; } = new List<GeneroDTO>();
        public List<CineDTO> Cines { get; set; } = new List<CineDTO>();
    }
}
