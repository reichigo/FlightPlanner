using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface IGetFlightSummaryUseCase
{
    Task<FlightSummaryResponse> ExecuteAsync(CancellationToken ct);
}

