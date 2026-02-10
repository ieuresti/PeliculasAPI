using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Servicios;

namespace PeliculasAPI.Controllers
{
    [Route("api/peliculas")]
    [ApiController]
    public class PeliculaController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private const string cacheTag = "peliculas";
        private readonly string contenedor = "peliculas";

        public PeliculaController(ApplicationDbContext context, IMapper mapper,
            IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos) : base(context, mapper, outputCacheStore, cacheTag)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet("PostGet")]
        public async Task<ActionResult<PeliculasPostGetDTO>> PostGet()
        {
            var generos = await context.Generos.ProjectTo<GeneroDTO>(mapper.ConfigurationProvider).ToListAsync();
            var cines = await context.Cines.ProjectTo<CineDTO>(mapper.ConfigurationProvider).ToListAsync();
            var respuesta = new PeliculasPostGetDTO
            {
                Generos = generos,
                Cines = cines
            };
            return respuesta;
        }

        [HttpGet("PutGet/{id:int}")]
        public async Task<ActionResult<PeliculasPutGetDTO>> PutGet(int id)
        {
            var pelicula = await context.Peliculas
                .ProjectTo<PeliculaDetallesDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pelicula is null)
            {
                return NotFound();
            }

            var generosSeleccionadosIds = pelicula.Generos.Select(g => g.Id).ToList(); // Obtener los IDs de los géneros seleccionados en la película
            var generosNoSeleccionados = await context.Generos
                .Where(g => !generosSeleccionadosIds.Contains(g.Id))
                .ProjectTo<GeneroDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            var cinesSeleccionadosIds = pelicula.Cines.Select(c => c.Id).ToList(); // Obtener los IDs de los cines seleccionados en la película
            var cinesNoSeleccionados = await context.Cines
                .Where(c => !cinesSeleccionadosIds.Contains(c.Id))
                .ProjectTo<CineDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            var respuesta = new PeliculasPutGetDTO();
            respuesta.Pelicula = pelicula;
            respuesta.GenerosSeleccionados = pelicula.Generos;
            respuesta.GenerosNoSeleccionados = generosNoSeleccionados;
            respuesta.CinesSeleccionados = pelicula.Cines;
            respuesta.CinesNoSeleccionados = cinesNoSeleccionados;
            respuesta.Actores = pelicula.Actores;

            return respuesta;
        }

        [HttpGet("landing")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<LandingPageDTO>> Get()
        {
            var top = 6;
            var hoy = DateTime.Today;

            var proximosEstrenos = await context.Peliculas
                .Where(p => p.FechaLanzamiento > hoy)
                .OrderBy(p => p.FechaLanzamiento)
                .Take(top)
                .ProjectTo<PeliculaDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            var enCines = await context.Peliculas
                .Where(p => p.PeliculaCines.Select(pc => pc.PeliculaId).Contains(p.Id)) // Peliculas que tienen una relación con PeliculaCines, es decir, que están en cines
                .OrderBy(p => p.FechaLanzamiento)
                .Take(top)
                .ProjectTo<PeliculaDTO>(mapper.ConfigurationProvider)
                .ToListAsync();

            var respuesta = new LandingPageDTO();
            respuesta.EnCines = enCines;
            respuesta.ProximosEstrenos = proximosEstrenos;

            return respuesta;
        }

        [HttpGet("{id:int}", Name = "ObtenerPeliculaPorId")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<ActionResult<PeliculaDetallesDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas
                .ProjectTo<PeliculaDetallesDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
            {
                return NotFound();
            }

            return pelicula;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO) // FromForm ya que se recibe un archivo a traves del IFormFile
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO); // mapper.Map<destino>(source)

            if (peliculaCreacionDTO.Poster is not null) // Si se envio un poster
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, peliculaCreacionDTO.Poster);
                pelicula.Poster = url;
            }
            AsignarOrdenActores(pelicula);

            context.Add(pelicula);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default); // Limpiar cache al crear una pelicula
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula); // Mapear a PeliculaDTO para retornarlo en la respuesta
            return CreatedAtRoute("ObtenerPeliculaPorId", new { id = pelicula.Id }, peliculaDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO) // FromForm ya que se recibe un archivo a traves del IFormFile
        {
            var pelicula = await context.Peliculas
                .Include(p => p.PeliculaActores) // Incluir la relación de PeliculaActores para poder actualizarla
                .Include(p => p.PeliculaCines) // Incluir la relación de PeliculaCines para poder actualizarla
                .Include(p => p.PeliculaGeneros) // Incluir la relación de PeliculaGeneros para poder actualizarla
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pelicula is null)
            {
                return NotFound();
            }

            pelicula = mapper.Map(peliculaCreacionDTO, pelicula); // Mapear los datos de peliculaCreacionDTO a la entidad pelicula existente
            if (peliculaCreacionDTO.Poster is not null) // Si se envio un nuevo poster
            {
                pelicula.Poster = await almacenadorArchivos.Editar(pelicula.Poster, contenedor, peliculaCreacionDTO.Poster);
            }

            AsignarOrdenActores(pelicula);

            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default); // Limpiar cache
            return NoContent(); // Todo esta ok, no hay que retornar ningun tipo de contenido
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await Delete<Pelicula>(id);
        }

        private void AsignarOrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculaActores is not null)
            {
                for (int i = 0; i < pelicula.PeliculaActores.Count; i++)
                {
                    pelicula.PeliculaActores[i].Orden = i;
                }
            }
        }
    }
}
