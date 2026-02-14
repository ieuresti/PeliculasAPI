namespace PeliculasAPI.DTOs
{
    // Esta clase se utiliza para retornar la respuesta de la autenticacion, que es el token JWT y su fecha de expiracion
    public class RespuestaAutenticacionDTO
    {
        public required string Token { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
