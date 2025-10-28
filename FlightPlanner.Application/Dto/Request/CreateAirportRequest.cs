namespace FlightPlanner.Application.Dto.Request;

public sealed record CreateAirportRequest(
    string Iata, 
    string Name, 
    double Latitude,
    double Longitude
    );