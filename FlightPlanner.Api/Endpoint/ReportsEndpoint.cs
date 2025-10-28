using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.UseCases;

namespace FlightPlanner.Api.Endpoint;

public static class ReportsEndpoint
{
    public static void MapReportsEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/reports")
            .WithTags("Reports")
            .WithOpenApi();

        group.MapGet("/", GetFlightReportAsync)
            .WithName("GetFlightReport")
            .Produces<FlightReportResponse>(StatusCodes.Status200OK)
            .WithOpenApi();

        group.MapGet("/summary", GetFlightSummaryAsync)
            .WithName("GetFlightSummary")
            .Produces<FlightSummaryResponse>(StatusCodes.Status200OK)
            .WithOpenApi();
    }

    private static async Task<IResult> GetFlightReportAsync(
        IGetFlightReportUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetFlightSummaryAsync(
        IGetFlightSummaryUseCase useCase,
        CancellationToken ct)
    {
        var result = await useCase.ExecuteAsync(ct);
        return Results.Ok(result);
    }
}

