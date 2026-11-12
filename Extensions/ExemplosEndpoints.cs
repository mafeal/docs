using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using docs.Dto;
using docs.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace docs.Extensions;

public static class ExemplosEndpoints
{
    public static RouteGroupBuilder MapExemplos(this IEndpointRouteBuilder app)
    {
        var exemplos = app.MapGroup("/exemplos");

        exemplos.MapGet("/", () =>
            listarExemplos());

        exemplos.MapGet("/{id:int:min(1)}", (int id) =>
            Buscar(id));
        
        exemplos.MapGet("/{termo:minlength(3)}", (string termo) =>
            BuscarPorTermo(termo));

        exemplos.MapPatch("/{id}", (int id, [FromQuery] string tipo) =>
            atualizaTipo(id, tipo));

        exemplos.MapPost("/", ([FromBody] ExemploDto exemplo) =>
            adicionaExemplo(exemplo));

        return exemplos;
    }

    // Métodos auxiliares

    public static IResult listarExemplos()
    {
        return Results.Ok(DocsRepository.bancoDados
                            .Select(p => new
                            {
                                Id = p.Key,
                                p.Value.Nome,
                                p.Value.Tipo
                            }));
    }

    public static IResult Buscar(int id)
    {
        if (DocsRepository.bancoDados.TryGetValue(id, out var exemplo))
        {
            return Results.Ok(exemplo);
        }

        return Results.NotFound("Não encontramos registro com esse Id.");
    }

    public static IResult BuscarPorTermo(string termo)
    {
        var exemplos = DocsRepository.bancoDados.Values
            .Where(p => p.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (exemplos is not null)
        {
            return Results.Ok(exemplos);
        }

        return Results.NotFound("Não encontramos exemplo com esse termo.");
    }

    public static IResult atualizaTipo(int id, string tipoNovo)
    {
        if (DocsRepository.bancoDados.TryGetValue(id, out var exemplo))
        {
            exemplo.Tipo = tipoNovo;
            return Results.Ok(exemplo);
        }

        return Results.NotFound("Não encontramos registro com esse Id.");
    }

    public static IResult adicionaExemplo(ExemploDto exemplo)
    {
        if (exemplo is null || exemplo.Nome is null || exemplo.Tipo is null)
            return Results.BadRequest("Os dados de entrada devem ser informados.");

        // próximo id: maior id existente + 1 (Count + 1 repetiria ids após um DELETE)
        var id = DocsRepository.bancoDados.Keys.DefaultIfEmpty(0).Max() + 1;
        DocsRepository.bancoDados.Add(id, exemplo);
        return Results.Created($"/exemplo/{@id}", exemplo);
    }
}


