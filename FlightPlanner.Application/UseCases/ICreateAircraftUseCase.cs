using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface ICreateAircraftUseCase
{
    Task<AircraftResponse> ExecuteAsync(CreateAircraftRequest request, CancellationToken ct);
}

