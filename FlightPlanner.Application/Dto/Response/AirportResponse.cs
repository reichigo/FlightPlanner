namespace FlightPlanner.Application.Dto.Response;

public sealed record AirportResponse(
    Guid Id,
    string Iata,
    string Name,
    double Latitude,
    double Longitude
);

