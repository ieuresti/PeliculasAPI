using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            ConfigurarMapeoGeneros();
            ConfigurarMapeoActores();
            ConfigurarMapeoCines(geometryFactory);
            ConfigurarMapeoPeliculas();
            ConfigurarMapeoUsuarios();
        }

        private void ConfigurarMapeoGeneros()
        {
            CreateMap<GeneroCreacionDTO, Genero>();

            CreateMap<Genero, GeneroDTO>();
        }

        private void ConfigurarMapeoActores()
        {
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x => x.Foto, opciones => opciones.Ignore()); // Ignorar mapeo automatico de esa propiedad entre un IFormFile y un string
            
            CreateMap<Actor, ActorDTO>();

            CreateMap<Actor, PeliculaActorDTO>();
        }

        private void ConfigurarMapeoCines(GeometryFactory geometryFactory)
        {
            CreateMap<Cine, CineDTO>()
                .ForMember(x => x.Latitud, cine => cine.MapFrom(p => p.Ubicacion.Y))
                .ForMember(x => x.Longitud, cine => cine.MapFrom(p => p.Ubicacion.X));

            CreateMap<CineCreacionDTO, Cine>()
                .ForMember(x => x.Ubicacion, cineDTO => cineDTO.MapFrom(p => 
                geometryFactory.CreatePoint(new Coordinate(p.Longitud, p.Latitud))));
        }

        private void ConfigurarMapeoPeliculas()
        {
            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(x => x.Poster, opciones => opciones.Ignore()) // Ignorar mapeo automatico de esa propiedad entre un IFormFile y un string
                .ForMember(x => x.PeliculaGeneros, dto => 
                    dto.MapFrom(p => p.GenerosIds!.Select(id => new PeliculaGenero { GeneroId = id }))) // Mapear lista de IDs a lista de entidades de relacion
                .ForMember(x => x.PeliculaCines, dto => 
                    dto.MapFrom(p => p.CinesIds!.Select(id => new PeliculaCine { CineId = id }))) // Mapear lista de IDs a lista de entidades de relacion
                .ForMember(x => x.PeliculaActores, dto =>
                    dto.MapFrom(p => p.Actores!.Select(a => new PeliculaActor { ActorId = a.Id, Personaje = a.Personaje }))); // Mapear lista de actores
            
            CreateMap<Pelicula, PeliculaDTO>();

            CreateMap<Pelicula, PeliculaDetallesDTO>()
                .ForMember(p => p.Generos, entidad => entidad.MapFrom(p => p.PeliculaGeneros))
                .ForMember(p => p.Cines, entidad => entidad.MapFrom(p => p.PeliculaCines))
                .ForMember(p => p.Actores, entidad => entidad.MapFrom(p => p.PeliculaActores.OrderBy(o => o.Orden)));

            CreateMap<PeliculaGenero, GeneroDTO>()
                .ForMember(g => g.Id, pg => pg.MapFrom(p => p.GeneroId))
                .ForMember(g => g.Nombre, pg => pg.MapFrom(p => p.Genero.Nombre));

            CreateMap<PeliculaCine, CineDTO>()
                .ForMember(c => c.Id, pc => pc.MapFrom(p => p.CineId))
                .ForMember(c => c.Nombre, pc => pc.MapFrom(p => p.Cine.Nombre))
                .ForMember(c => c.Latitud, pc => pc.MapFrom(p => p.Cine.Ubicacion.Y))
                .ForMember(c => c.Longitud, pc => pc.MapFrom(p => p.Cine.Ubicacion.X));

            CreateMap<PeliculaActor, PeliculaActorDTO>()
                .ForMember(a => a.Id, pa => pa.MapFrom(p => p.ActorId))
                .ForMember(a => a.Nombre, pa => pa.MapFrom(p => p.Actor.Nombre))
                .ForMember(a => a.Foto, pa => pa.MapFrom(p => p.Actor.Foto));
        }

        private void ConfigurarMapeoUsuarios()
        {
            CreateMap<IdentityUser, UsuarioDTO>();
        }
    }
}
