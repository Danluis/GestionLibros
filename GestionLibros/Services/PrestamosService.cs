using Aplicada1.Core;
using GestionLibros.Context;
using GestionLibros.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestionLibros.Services;

public class PrestamosService(
    IDbContextFactory<Contexto> DbFactory
) : Aplicada1.Core.IServiceResult<Prestamos, int>
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

    public async Task<Result> Guardar(Prestamos prestamo)
    {
        var esNuevo = !await Existe(prestamo.PrestamoId);

        if (esNuevo)
        {
            if (await EstudianteTienePrestamoActivo(prestamo.EstudianteId))
            {
                return Result.Failure(new Error("ESTUDIANTE_CON_PRESTAMO", "Este estudiante ya tiene un préstamo activo."));
            }

            if (await LibroEstaPrestado(prestamo.LibroId))
            {
                return Result.Failure(new Error("LIBRO_PRESTADO", "Este libro ya está prestado a otro estudiante."));
            }

            var insertado = await Insertar(prestamo);
            return Result.Success(insertado);
        }
        else
        {
            var modificado = await Modificar(prestamo);
            return Result.Success(modificado);
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

    public async Task<Result<Prestamos?>> Buscar(int prestamoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Prestamos
            .Include(p => p.Estudiante)
            .Include(p => p.Libro)
            .FirstOrDefaultAsync(p => p.PrestamoId == prestamoId);
    }

    public async Task<Result> Eliminar(int prestamoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var eliminado = await contexto.Prestamos
            .AsNoTracking()
            .Where(p => p.PrestamoId == prestamoId)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }

    public async Task<Result<List<Prestamos>>> GetList(Expression<Func<Prestamos, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var lista = await contexto.Prestamos
            .Include(p => p.Estudiante)
            .Include(p => p.Libro)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
        return Result.Success(lista);
    }

    // --- Devolución: registra en el historial y cierra el préstamo ---
    public async Task<Result> Devolver(int prestamoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var prestamo = await contexto.Prestamos.FirstOrDefaultAsync(p => p.PrestamoId == prestamoId);

        if (prestamo is null)
        {
            return Result.Failure(new Error("PRESTAMO_NO_ENCONTRADO", "El préstamo no existe."));
        }

        if (!prestamo.Activo)
        {
            return Result.Failure(new Error("PRESTAMO_YA_DEVUELTO", "Este préstamo ya fue devuelto."));
        }

        prestamo.Activo = false;

        contexto.Devoluciones.Add(new Devolucion
        {
            PrestamoId = prestamo.PrestamoId,
            FechaDevolucion = DateTime.Now
        });

        var guardado = await contexto.SaveChangesAsync() > 0;
        return Result.Success(guardado);
    }

    public async Task<Result<List<Devolucion>>> GetDevoluciones(Expression<Func<Devolucion, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var lista = await contexto.Devoluciones
            .Include(d => d.Prestamo!.Libro)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
        return Result.Success(lista);
    }
}
