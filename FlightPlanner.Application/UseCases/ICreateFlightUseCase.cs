using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface ICreateFlightUseCase
{
    Task<FlightResponse> ExecuteAsync(CreateFlightRequest request, CancellationToken ct);
}

