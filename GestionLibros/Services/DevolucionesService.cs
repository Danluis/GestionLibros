using Aplicada1.Core;
using GestionLibros.Context;
using GestionLibros.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestionLibros.Services;

public class DevolucionesService(
    IDbContextFactory<Contexto> DbFactory
) : Aplicada1.Core.IServiceResult<Devolucion, int>
{
    public async Task<Result> Guardar(Devolucion devolucion)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var prestamo = await contexto.Prestamos.FirstOrDefaultAsync(p => p.PrestamoId == devolucion.PrestamoId);

        if (prestamo is null)
        {
            return Result.Failure(new Error("PRESTAMO_NO_ENCONTRADO", "El préstamo no existe."));
        }

        if (!prestamo.Activo)
        {
            return Result.Failure(new Error("PRESTAMO_YA_DEVUELTO", "Este préstamo ya fue devuelto."));
        }

        prestamo.Activo = false;
        devolucion.FechaDevolucion = DateTime.Now;
        contexto.Devoluciones.Add(devolucion);

        var guardado = await contexto.SaveChangesAsync() > 0;
        return Result.Success(guardado);
    }

    public async Task<Result<Devolucion?>> Buscar(int devolucionId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Devoluciones
            .Include(d => d.Prestamo!.Libro)
            .Include(d => d.Prestamo!.Estudiante)
            .FirstOrDefaultAsync(d => d.DevolucionId == devolucionId);
    }

    public async Task<Result> Eliminar(int devolucionId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var eliminado = await contexto.Devoluciones
            .AsNoTracking()
            .Where(d => d.DevolucionId == devolucionId)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }

    public async Task<Result<List<Devolucion>>> GetList(Expression<Func<Devolucion, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var lista = await contexto.Devoluciones
            .Include(d => d.Prestamo!.Libro)
            .Include(d => d.Prestamo!.Estudiante)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
        return Result.Success(lista);
    }
}
