using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface IGetAirportUseCase
{
    Task<AirportResponse?> ExecuteAsync(Guid id, CancellationToken ct);
}

