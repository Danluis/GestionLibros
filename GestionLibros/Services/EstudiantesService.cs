using GestionLibros.Context;
using GestionLibros.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestionLibros.Services
{
    public class EstudiantesService(
        IDbContextFactory<Contexto> DbFactory
    ) : Aplicada1.Core.IService<Estudiantes, int>
    {
        public async Task<bool> NombreExiste(string nombres, int estudianteIdActual)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Estudiantes
                .AnyAsync(e => e.Nombres == nombres && e.EstudianteId != estudianteIdActual);
        }

        public async Task<bool> Guardar(Estudiantes estudiante)
        {
            if (await NombreExiste(estudiante.Nombres, estudiante.EstudianteId))
            {
                throw new InvalidOperationException("Ya existe un estudiante con ese nombre.");
            }

            if (!await Existe(estudiante.EstudianteId))
            {
                return await Insertar(estudiante);
            }
            else
            {
                return await Modificar(estudiante);
            }
        }

        private async Task<bool> Existe(int estudianteId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Estudiantes
                .AnyAsync(e => e.EstudianteId == estudianteId);
        }

        private async Task<bool> Insertar(Estudiantes estudiante)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Estudiantes.Add(estudiante);
            return await contexto.SaveChangesAsync() > 0;
        }

        private async Task<bool> Modificar(Estudiantes estudiante)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Estudiantes.Update(estudiante);
            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<Estudiantes?> Buscar(int estudianteId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Estudiantes
                .FirstOrDefaultAsync(e => e.EstudianteId == estudianteId);
        }

        public async Task<bool> Eliminar(int estudianteId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Estudiantes
                .AsNoTracking()
                .Where(e => e.EstudianteId == estudianteId)
                .ExecuteDeleteAsync() > 0;
        }

        public async Task<List<Estudiantes>> GetList(Expression<Func<Estudiantes, bool>> criterio)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Estudiantes
                .Where(criterio)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}