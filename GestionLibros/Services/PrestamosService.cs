using GestionLibros.Context;
using GestionLibros.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestionLibros.Services
{
    public class PrestamosService(
        IDbContextFactory<Contexto> DbFactory
    ) : Aplicada1.Core.IService<Prestamos, int>
    {
        public async Task<bool> EstudianteTienePrestamoActivo(int estudianteId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Prestamos
                .AnyAsync(p => p.EstudianteId == estudianteId && p.Activo);
        }

        public async Task<bool> LibroEstaPrestado(int libroId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Prestamos
                .AnyAsync(p => p.LibroId == libroId && p.Activo);
        }

        public async Task<bool> Guardar(Prestamos prestamo)
        {
            var esNuevo = !await Existe(prestamo.PrestamoId);

            if (esNuevo)
            {
                if (await EstudianteTienePrestamoActivo(prestamo.EstudianteId))
                {
                    throw new InvalidOperationException("Este estudiante ya tiene un préstamo activo.");
                }

                if (await LibroEstaPrestado(prestamo.LibroId))
                {
                    throw new InvalidOperationException("Este libro ya está prestado a otro estudiante.");
                }

                return await Insertar(prestamo);
            }
            else
            {
                return await Modificar(prestamo);
            }
        }

        private async Task<bool> Existe(int prestamoId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Prestamos
                .AnyAsync(p => p.PrestamoId == prestamoId);
        }

        private async Task<bool> Insertar(Prestamos prestamo)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Prestamos.Add(prestamo);
            return await contexto.SaveChangesAsync() > 0;
        }

        private async Task<bool> Modificar(Prestamos prestamo)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Prestamos.Update(prestamo);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<Prestamos?> Buscar(int prestamoId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Prestamos
                .Include(p => p.Estudiante)
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.PrestamoId == prestamoId);
        }

        public async Task<bool> Eliminar(int prestamoId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Prestamos
                .AsNoTracking()
                .Where(p => p.PrestamoId == prestamoId)
                .ExecuteDeleteAsync() > 0;
        }

        public async Task<List<Prestamos>> GetList(Expression<Func<Prestamos, bool>> criterio)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Prestamos
                .Include(p => p.Estudiante)
                .Include(p => p.Libro)
                .Where(criterio)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}