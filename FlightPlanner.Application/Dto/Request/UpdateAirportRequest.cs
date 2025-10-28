namespace FlightPlanner.Application.Dto.Request;

public sealed record UpdateAirportRequest(
    string Iata,
    string Name,
    double Latitude,
    double Longitude
);

