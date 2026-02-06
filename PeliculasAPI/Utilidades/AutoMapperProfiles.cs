using AutoMapper;
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
        }
    }
}
