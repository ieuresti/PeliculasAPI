using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Entidades
{
    public class Actor : IId
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(150, ErrorMessage = "El campo {0} debe contener {1} caracteres o menos")]
        [PrimeraLetraMayuscula]
        public required string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        [Unicode(false)] // Va a ser una URL y no se necesitan todos los caracteres Unicode (varchar y no nvarchar)
        public string? Foto { get; set; }
    }
}
