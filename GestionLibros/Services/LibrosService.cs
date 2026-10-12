using Aplicada1.Core;
using GestionLibros.Context;
using GestionLibros.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GestionLibros.Services;


public class LibrosService(
    IDbContextFactory<Contexto> DbFactory
) : Aplicada1.Core.IServiceResult<Libros, int>
{
    public async Task<bool> TituloExiste(string titulo, int libroIdActual)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Libros
            .AnyAsync(l => l.Titulo == titulo && l.LibroId != libroIdActual);
    }

    public async Task<Result> Guardar(Libros libro)
    {
        if (await TituloExiste(libro.Titulo, libro.LibroId))
        {
            return Result.Failure(new Error("TITULO_DUPLICADO", "Ya existe un libro con ese título."));
        }

        if (!await Existe(libro.LibroId))
        {
            var insertado = await Insertar(libro);
            return Result.Success(insertado);
        }
        else
        {
            var modificado = await Modificar(libro);
            return Result.Success(modificado);
        }
    }

    private async Task<bool> Existe(int libroId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Libros
            .AnyAsync(l => l.LibroId == libroId);
    }

    private async Task<bool> Insertar(Libros libro)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Libros.Add(libro);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Libros libro)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Libros.Update(libro);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<Result<Libros?>> Buscar(int libroId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Libros
            .FirstOrDefaultAsync(l => l.LibroId == libroId);
    }

    public async Task<Result> Eliminar(int libroId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        var eliminado = await contexto.Libros
            .AsNoTracking()
            .Where(l => l.LibroId == libroId)
            .ExecuteDeleteAsync() > 0;
        return Result.Success(eliminado);
    }

    public async Task<Result<List<Libros>>> GetList(Expression<Func<Libros, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Libros
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}
