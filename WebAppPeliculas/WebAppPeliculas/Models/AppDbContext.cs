using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppPeliculas.Models;

namespace WebAppPeliculas.Models
{
    public class AppDBcontext : DbContext
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb; Database=Labo4; Trusted_Connection=True; MultipleActiveResultSets=True");
        //}

        // inyección de dependencia SQL
        public AppDBcontext(DbContextOptions<AppDBcontext> options)
        : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relación UNO A MUCHOS
            modelBuilder.Entity<Pelicula>()
                .HasOne(l => l.Genero)
                .WithMany(g => g.Peliculas)
                .HasForeignKey(l => l.IdGenero);

            // Relación MUCHOS A MUCHOS
            modelBuilder.Entity<PeliculaActor>()
                .HasKey(lc => new { lc.IdPelicula, lc.IdActor });

            modelBuilder.Entity<PeliculaActor>()
                .HasOne(lc => lc.Pelicula)
                .WithMany(l => l.PeliculasActores)
                .HasForeignKey(lc => lc.IdPelicula);

            modelBuilder.Entity<PeliculaActor>()
                .HasOne(lc => lc.Actor)
                .WithMany(c => c.PeliculasActores)
                .HasForeignKey(lc => lc.IdActor);
        }

        //Tablas
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<PeliculaActor> PeliculaActor { get; set; }
    }
}


