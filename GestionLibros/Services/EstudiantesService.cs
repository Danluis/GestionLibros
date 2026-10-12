using Aplicada1.Core;
using GestionLibros.Context;
using GestionLibros.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestionLibros.Services;

public class EstudiantesService(
    IDbContextFactory<Contexto> DbFactory
) : Aplicada1.Core.IServiceResult<Estudiantes, int>
{
    public async Task<bool> NombreExiste(string nombres, int estudianteIdActual)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .AnyAsync(e => e.Nombres == nombres && e.EstudianteId != estudianteIdActual);
    }

    public async Task<Result> Guardar(Estudiantes estudiante)
    {
        if (await NombreExiste(estudiante.Nombres, estudiante.EstudianteId))
        {
            return Result.Failure(new Error("NOMBRE_DUPLICADO", "Ya existe un estudiante con ese nombre."));
        }

        if (!await Existe(estudiante.EstudianteId))
        {
            var insertado = await Insertar(estudiante);
            return Result.Success(insertado);
        }
        else
        {
            var modificado = await Modificar(estudiante);
            return Result.Success(modificado);
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

    public async Task<Result<Estudiantes?>> Buscar(int estudianteId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .FirstOrDefaultAsync(e => e.EstudianteId == estudianteId);
    }

    public async Task<Result> Eliminar(int estudianteId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var eliminado = await contexto.Estudiantes
            .AsNoTracking()
            .Where(e => e.EstudianteId == estudianteId)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }

    public async Task<Result<List<Estudiantes>>> GetList(Expression<Func<Estudiantes, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Estudiantes
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}
