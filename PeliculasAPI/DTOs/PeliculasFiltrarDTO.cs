namespace PeliculasAPI.DTOs
{
    public class PeliculasFiltrarDTO
    {
        // Propiedades para filtrar y paginar peliculas
        public int Pagina { get; set; } = 1;
        public int RecordsPorPagina { get; set; } = 10;
        internal PaginacionDTO Paginacion
        {
            get
            {
                return new PaginacionDTO() { Pagina = Pagina, RecordsPorPagina = RecordsPorPagina };
            }
        }

        public string? Titulo { get; set; }
        public int GeneroId { get; set; }
        public bool EnCines { get; set; }
        public bool ProximosEstrenos { get; set; }
    }
}
