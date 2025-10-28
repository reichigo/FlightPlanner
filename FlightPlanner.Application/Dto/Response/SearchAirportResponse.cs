namespace FlightPlanner.Application.Dto.Response;

public record SearchAirportResponse(
    Guid Id, 
    string Model, 
    double CruiseSpeedKts,
    double FuelBurnPerHourKg, 
    double TakeoffFuelKg
    );