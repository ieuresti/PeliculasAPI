using Microsoft.EntityFrameworkCore;

namespace PeliculasAPI.DTOs
{
    public class PeliculaDTO
    {
        public int Id { get; set; }
        public required string Titulo { get; set; }
        public string? Trailer { get; set; }
        public DateTime FechaLanzamiento { get; set; }
        [Unicode(false)] // Va a ser una URL y no se necesitan todos los caracteres Unicode (varchar y no nvarchar)
        public string? Poster { get; set; }
    }
}
