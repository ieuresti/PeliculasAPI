namespace PeliculasAPI.DTOs
{
    // DTO para obtener los datos necesarios para crear o editar una pelicula
    public class PeliculasPutGetDTO
    {
        public PeliculaDTO Pelicula { get; set; } = null!;
        public List<GeneroDTO> GenerosSeleccionados { get; set; } = new List<GeneroDTO>();
        public List<GeneroDTO> GenerosNoSeleccionados { get; set; } = new List<GeneroDTO>();
        public List<CineDTO> CinesSeleccionados { get; set; } = new List<CineDTO>();
        public List<CineDTO> CinesNoSeleccionados { get; set; } = new List<CineDTO>();
        public List<PeliculaActorDTO> Actores { get; set; } = new List<PeliculaActorDTO>();
    }
}
