using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface IGetAircraftUseCase
{
    Task<AircraftResponse?> ExecuteAsync(Guid id, CancellationToken ct);
}

