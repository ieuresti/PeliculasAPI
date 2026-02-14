using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    // Esta clase se utiliza para recibir las credenciales del usuario (email y password) para el proceso de autenticacion
    public class CredencialesUsuarioDTO
    {
        [EmailAddress]
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
