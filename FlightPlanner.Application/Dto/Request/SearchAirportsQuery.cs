namespace FlightPlanner.Application.Dto.Request;

public sealed record SearchAirportsQuery(
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 10
);

