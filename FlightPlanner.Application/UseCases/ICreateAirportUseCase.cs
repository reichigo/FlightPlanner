using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface ICreateAirportUseCase
{
    Task<AirportResponse> ExecuteAsync(CreateAirportRequest request, CancellationToken ct);
}