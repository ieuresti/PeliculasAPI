using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PeliculasAPI.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PeliculasAPI.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;

        public UsuariosController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            // Crear un nuevo usuario con el email y password proporcionados
            var usuario = new IdentityUser
            {
                Email = credencialesUsuarioDTO.Email,
                UserName = credencialesUsuarioDTO.Email
            };

            // Intentar crear el usuario en la base de datos
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuarioDTO.Password);

            if (resultado.Succeeded)
            {
                // Si el usuario se creó correctamente, construir y retornar el token de autenticación
                return await ConstruirToken(usuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            // Buscar el usuario en la base de datos por su email
            var usuario = await userManager.FindByEmailAsync(credencialesUsuarioDTO.Email);

            if (usuario == null)
            {
                var errores = ConstruirLoginIncorrecto();
                return BadRequest(errores);
            }

            // Verificar que la contraseña proporcionada sea correcta para el usuario encontrado
            var resultado = await signInManager.CheckPasswordSignInAsync(usuario, credencialesUsuarioDTO.Password, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                // Si el login fue exitoso, construir y retornar el token de autenticación
                return await ConstruirToken(usuario);
            }
            else
            {
                var errores = ConstruirLoginIncorrecto();
                return BadRequest(errores);
            }
        }

        private async Task<RespuestaAutenticacionDTO> ConstruirToken(IdentityUser identityUser)
        {
            // Agregar los claims que se quieran incluir en el token
            var claims = new List<Claim>
            {
                new Claim("email", identityUser.Email!),
                new Claim("id", identityUser.Id)
            };

            // Agregar los claims que se tengan en la base de datos
            var claimsDB = await userManager.GetClaimsAsync(identityUser);

            // Agregar los claims de la base de datos a los claims que se van a incluir en el token
            claims.AddRange(claimsDB);

            // Crear la llave y credenciales que se van a usar para firmar el token
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]!));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            // Definir la expiración del token
            var expiracion = DateTime.UtcNow.AddYears(1);

            // Crear el token de seguridad
            var tokenDeSeguridad = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: credenciales);

            // Escribir el token de seguridad en formato string
            var token = new JwtSecurityTokenHandler().WriteToken(tokenDeSeguridad);

            // Retornar el token y la fecha de expiración
            return new RespuestaAutenticacionDTO
            {
                Token = token,
                Expiracion = expiracion
            };
        }

        private IEnumerable<IdentityError> ConstruirLoginIncorrecto()
        {
            // Construir un error de identidad personalizado para indicar que el login fue incorrecto
            var identityError = new IdentityError
            {
                Code = "LoginIncorrecto",
                Description = "Usuario o contraseña incorrectos"
            };
            // Retornar una lista con el error de identidad personalizado
            var errores = new List<IdentityError>();
            errores.Add(identityError);
            return errores;
        }
    }
}
