using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface IUpdateAirportUseCase
{
    Task<AirportResponse?> ExecuteAsync(Guid id, UpdateAirportRequest request, CancellationToken ct);
}

