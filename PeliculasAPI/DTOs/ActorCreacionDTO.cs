using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class ActorCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} debe contener {1} caracteres o menos")]
        [PrimeraLetraMayuscula]
        public required string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public IFormFile? Foto { get; set; } // Para la creación se recibe un archivo como tal o no
    }
}
