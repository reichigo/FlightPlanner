namespace FlightPlanner.Application.Dto.Response;

public sealed record AircraftResponse(
    Guid Id,
    string Model,
    double CruiseSpeedKts,
    double FuelBurnPerHourKg,
    double TakeoffFuelKg
);

