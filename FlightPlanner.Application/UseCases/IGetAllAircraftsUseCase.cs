using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface IGetAllAircraftsUseCase
{
    Task<List<AircraftResponse>> ExecuteAsync(CancellationToken ct);
}

