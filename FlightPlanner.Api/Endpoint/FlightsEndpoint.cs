using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Api.Endpoint;

public static class FlightsEndpoint
{
    public static void MapFlightsEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/flights")
            .WithTags("Flights")
            .WithOpenApi();

        group.MapPost("/", CreateFlightAsync)
            .WithName("CreateFlight")
            .Accepts<CreateFlightRequest>("application/json")
            .Produces<FlightResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        group.MapGet("/", GetAllFlightsAsync)
            .WithName("GetAllFlights")
            .Produces<List<FlightResponse>>(StatusCodes.Status200OK)
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetFlightAsync)
            .WithName("GetFlight")
            .Produces<FlightResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi();

        group.MapPut("/{id:guid}", UpdateFlightAsync)
            .WithName("UpdateFlight")
            .Accepts<UpdateFlightRequest>("application/json")
            .Produces<FlightResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi();

        group.MapDelete("/{id:guid}", DeleteFlightAsync)
            .WithName("DeleteFlight")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }

    private static async Task<IResult> CreateFlightAsync(
        [FromBody] CreateFlightRequest request,
        ICreateFlightUseCase useCase,
        CancellationToken ct)
    {
        // DEBUG: Log the request received
        Console.WriteLine($"DEBUG API - OriginAirportId: {request.OriginAirportId}");
        Console.WriteLine($"DEBUG API - DestinationAirportId: {request.DestinationAirportId}");
        Console.WriteLine($"DEBUG API - AircraftId: {request.AircraftId}");

        var result = await useCase.ExecuteAsync(request, ct);
        return Results.Created($"/flights/{result.Id}", result);
    }

    private static async Task<IResult> GetAllFlightsAsync(
        IGetAllFlightsUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetFlightAsync(
        Guid id,
        IGetFlightUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(id, ct);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }

    private static async Task<IResult> UpdateFlightAsync(
        Guid id,
        [FromBody] UpdateFlightRequest request,
        IUpdateFlightUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(id, request, ct);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }

    private static async Task<IResult> DeleteFlightAsync(
        Guid id,
        IDeleteFlightUseCase useCase,
        CancellationToken ct)
    {
        var deleted = await useCase.ExecuteAsync(id, ct);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}

