using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Utilidades;
using System.Linq.Expressions;

namespace PeliculasAPI.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IOutputCacheStore outputCacheStore;
        private readonly string cacheTag;

        public CustomBaseController(ApplicationDbContext context, IMapper mapper, IOutputCacheStore outputCacheStore, string cacheTag)
        {
            this.context = context;
            this.mapper = mapper;
            this.outputCacheStore = outputCacheStore;
            this.cacheTag = cacheTag;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>(
            Expression<Func<TEntidad, object>> ordenarPor) where TEntidad : class
        {
            return await context.Set<TEntidad>()
                .OrderBy(ordenarPor)
                .ProjectTo<TDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>(PaginacionDTO paginacion,
            Expression<Func<TEntidad, object>> ordenarPor) where TEntidad : class
        {
            var queryable = context.Set<TEntidad>().AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            // Hacer una proyeccion a <T>DTO para retornar la lista de dicho tipo (Ej Actor -> ActorDTO)
            return await queryable
                .OrderBy(ordenarPor)
                .Paginar(paginacion)
                .ProjectTo<TDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id)
            where TEntidad : class, IId
            where TDTO : IId
        {
            var entidad = await context.Set<TEntidad>()
                .ProjectTo<TDTO>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (entidad is null)
            {
                return NotFound();
            }
            return entidad;
        }

        protected async Task<IActionResult> Post<TCreacionDTO, TEntidad, TDTO>(TCreacionDTO creacionDTO, string nombreRuta)
            where TEntidad : class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacionDTO); // mapper.Map<destino>(source)
            context.Add(entidad);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default); // Limpiar cache al crear una entidad
            var entidadDTO = mapper.Map<TDTO>(entidad);
            return CreatedAtRoute(nombreRuta, new { id = entidad.Id }, entidadDTO);
        }

        protected async Task<IActionResult> Put<TCreacionDTO, TEntidad>(int id, TCreacionDTO creacionDTO)
            where TEntidad : class, IId
        {
            var entidadExiste = await context.Set<TEntidad>().AnyAsync(e => e.Id == id);
            if (!entidadExiste)
            {
                return NotFound();
            }

            var entidad = mapper.Map<TEntidad>(creacionDTO);
            entidad.Id = id;

            context.Update(entidad);
            await context.SaveChangesAsync();
            await outputCacheStore.EvictByTagAsync(cacheTag, default); // Limpiar cache al actualizar una entidad
            return NoContent(); // Todo esta ok, no hay que retornar ningun tipo de contenido
        }

        protected async Task<IActionResult> Delete<TEntidad>(int id)
            where TEntidad : class, IId
        {
            // Todos los registros que cumplan con la condicion del Where seran borrados
            var registrosBorrados = await context.Set<TEntidad>().Where(e => e.Id == id).ExecuteDeleteAsync();
            if (registrosBorrados == 0)
            {
                // Si es 0 quiere decir que no existe un registro con dicho id
                return NotFound();
            }

            await outputCacheStore.EvictByTagAsync(cacheTag, default); // Limpiar cache al borrar una entidad
            return NoContent();
        }
    }
}
