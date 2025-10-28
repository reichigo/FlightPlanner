using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface IGetFlightUseCase
{
    Task<FlightResponse?> ExecuteAsync(Guid id, CancellationToken ct);
}

