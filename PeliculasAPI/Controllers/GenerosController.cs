using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
    [Route("api/generos")]
    [ApiController]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private const string cacheTag = "generos";

        public GenerosController(ApplicationDbContext context, IMapper mapper, IOutputCacheStore outputCacheStore) {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
        }

        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public List<GeneroDTO> Get()
        {
            return new List<GeneroDTO>() { new GeneroDTO { Id = 1, Nombre = "Comedia" }, new GeneroDTO { Id = 2, Nombre = "Acción" } };
        }

        [HttpGet("{id}", Name = "ObtenerGeneroPorId")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<GeneroDTO>> Get(int id) 
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var genero = mapper.Map<Genero>(generoCreacionDTO);
            context.Add(genero);
            await context.SaveChangesAsync();
            return CreatedAtRoute("ObtenerGeneroPorId", new {id = genero.Id}, genero);
        }

        [HttpPut]
        public void Put()
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public void Delete()
        {
            throw new NotImplementedException();
        }
    }
}
