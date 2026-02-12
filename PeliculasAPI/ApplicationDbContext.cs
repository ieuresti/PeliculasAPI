using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Entidades;

namespace PeliculasAPI
{
    public class ApplicationDbContext : IdentityDbContext // IdentityDbContext se utiliza para manejar la autenticacion y autorizacion de usuarios, roles, etc.
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // OnModelCreating se utiliza para configurar el modelo de datos, como las relaciones entre entidades, claves primarias, etc.
            base.OnModelCreating(modelBuilder); // Si esto no esta, no se aplicaran las configuraciones de IdentityDbContext

            modelBuilder.Entity<PeliculaGenero>()
                .HasKey(e => new { e.GeneroId, e.PeliculaId }); // Configurar clave primaria compuesta
            modelBuilder.Entity<PeliculaCine>()
                .HasKey(e => new { e.CineId, e.PeliculaId }); // Configurar clave primaria compuesta
            modelBuilder.Entity<PeliculaActor>()
                .HasKey(e => new { e.ActorId, e.PeliculaId }); // Configurar clave primaria compuesta
        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Cine> Cines { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<PeliculaGenero> PeliculaGeneros { get; set; }
        public DbSet<PeliculaCine> PeliculaCines { get; set; }
        public DbSet<PeliculaActor> PeliculaActores { get; set; }
    }
}
