using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Api.Endpoint;

public static class AircraftsEndpoint
{
    public static void MapAircraftsEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/aircrafts")
            .WithTags("Aircrafts")
            .WithOpenApi();

        group.MapPost("/", CreateAircraftAsync)
            .WithName("CreateAircraft")
            .Accepts<CreateAircraftRequest>("application/json")
            .Produces<AircraftResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        group.MapGet("/", GetAllAircraftsAsync)
            .WithName("GetAllAircrafts")
            .Produces<List<AircraftResponse>>(StatusCodes.Status200OK)
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetAircraftAsync)
            .WithName("GetAircraft")
            .Produces<AircraftResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }

    private static async Task<IResult> CreateAircraftAsync(
        [FromBody] CreateAircraftRequest request,
        ICreateAircraftUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(request, ct);
        return Results.Created($"/aircrafts/{result.Id}", result);
    }

    private static async Task<IResult> GetAllAircraftsAsync(
        IGetAllAircraftsUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAircraftAsync(
        Guid id,
        IGetAircraftUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(id, ct);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}

