using GestionLibros.DAL;
using GestionLibros.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestionLibros.Services
{
    public class LibrosService(IDbContextFactory<Contexto> DbFactory)
    {

        public async Task<bool> TituloExiste(string titulo, int libroIdActual)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Libros
                .AnyAsync(l => l.Titulo == titulo && l.LibroId != libroIdActual);
        }

        public async Task<bool> Guardar(Libro libro)
        {
            if (await TituloExiste(libro.Titulo, libro.LibroId))
            {
                throw new InvalidOperationException("Ya existe un libro con ese título.");
            }

            if (!await Existe(libro.LibroId))
            {
                return await Insertar(libro);
            }
            else
            {
                return await Modificar(libro);
            }
        }

        private async Task<bool> Existe(int libroId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Libros
                .AnyAsync(l => l.LibroId == libroId);
        }
        private async Task<bool> Insertar(Libro libro)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Libros.Add(libro);
            return await contexto.SaveChangesAsync() > 0;
        }

        private async Task<bool> Modificar(Libro libro)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Libros.Update(libro);
            return await contexto.
                SaveChangesAsync() > 0;
        }

        public async Task<Libro?> Buscar(int libroId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Libros
                .FirstOrDefaultAsync(l => l.LibroId == libroId);
        }

        public async Task<bool> Eliminar(int libroId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Libros.
                AsNoTracking()
                .Where(l => l.LibroId == libroId)
                .ExecuteDeleteAsync() > 0;
        }

        public async Task<List<Libro>> Listar(Expression<Func<Libro, bool>> criterio) 
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Libros
                .Where(criterio)
                .AsNoTracking()
                .ToListAsync();
        }
    }
} 