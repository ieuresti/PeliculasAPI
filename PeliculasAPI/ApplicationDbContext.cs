using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Entidades;

namespace PeliculasAPI
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Llama al metodo base para asegurar que la configuracion predeterminada se aplique

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
