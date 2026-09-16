using GestionLibros.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionLibros.Context
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) { }

        public DbSet<Libros> Libros { get; set; } = null!;
    }
}