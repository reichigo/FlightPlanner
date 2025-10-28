using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.Mappers;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class SearchAirportsUseCase(IAirportRepository airportRepository) : ISearchAirportsUseCase
{
    public async Task<PagedResponse<AirportResponse>> ExecuteAsync(SearchAirportsQuery query, CancellationToken ct)
    {
        var skip = (query.Page - 1) * query.PageSize;
        var airports = await airportRepository.SearchAsync(query.SearchTerm ?? "", skip, query.PageSize, ct);
        var totalCount = await airportRepository.CountAsync(query.SearchTerm, ct);
        var totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        var items = airports.Select(a => a.ToResponse()).ToList();

        return new PagedResponse<AirportResponse>(
            items,
            query.Page,
            query.PageSize,
            totalCount,
            totalPages
        );
    }
}

