using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface IGetFlightReportUseCase
{
    Task<FlightReportResponse> ExecuteAsync(CancellationToken ct);
}

