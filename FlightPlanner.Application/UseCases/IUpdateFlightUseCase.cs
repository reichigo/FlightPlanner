using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface IUpdateFlightUseCase
{
    Task<FlightResponse?> ExecuteAsync(Guid id, UpdateFlightRequest request, CancellationToken ct);
}

