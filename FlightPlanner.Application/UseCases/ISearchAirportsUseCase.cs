using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;

namespace FlightPlanner.Application.UseCases;

public interface ISearchAirportsUseCase
{
    Task<PagedResponse<AirportResponse>> ExecuteAsync(SearchAirportsQuery query, CancellationToken ct);
}

