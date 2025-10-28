using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Api.Endpoint;

public static class AirportsEndpoint
{
    public static void MapAirportsEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/airports")
            .WithTags("Airports")
            .WithOpenApi();

        group.MapPost("/", CreateAirportAsync)
            .WithName("CreateAirport")
            .Accepts<CreateAirportRequest>("application/json")
            .Produces<AirportResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        group.MapGet("/search", SearchAirportsAsync)
            .WithName("SearchAirports")
            .Produces<PagedResponse<AirportResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetAirportAsync)
            .WithName("GetAirport")
            .Produces<AirportResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi();

        group.MapPut("/{id:guid}", UpdateAirportAsync)
            .WithName("UpdateAirport")
            .Accepts<UpdateAirportRequest>("application/json")
            .Produces<AirportResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi();
    }

    private static async Task<IResult> CreateAirportAsync(
        [FromBody] CreateAirportRequest request,
        ICreateAirportUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(request, ct);
        return Results.Created($"/airports/{result.Id}", result);
    }

    private static async Task<IResult> SearchAirportsAsync(
        [AsParameters] SearchAirportsQuery query,
        ISearchAirportsUseCase useCase,
        CancellationToken ct)
    {
        if (query.Page <= 0 || query.PageSize is < 1 or > 200)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["page"] = new[] { "Must be >= 1" },
                ["pageSize"] = new[] { "Must be between 1 and 200" }
            });
        }

        var result = await useCase.ExecuteAsync(query, ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAirportAsync(
        Guid id,
        IGetAirportUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(id, ct);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }

    private static async Task<IResult> UpdateAirportAsync(
        Guid id,
        [FromBody] UpdateAirportRequest request,
        IUpdateAirportUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(id, request, ct);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}