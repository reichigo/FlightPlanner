using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface IGetAllFlightsUseCase
{
    Task<List<FlightResponse>> ExecuteAsync(CancellationToken ct);
}

