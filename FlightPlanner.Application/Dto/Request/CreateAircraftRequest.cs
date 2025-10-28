namespace FlightPlanner.Application.Dto.Request;

public sealed record CreateAircraftRequest(
    string Model,
    double CruiseSpeedKts,
    double FuelBurnPerHourKg,
    double TakeoffFuelKg
);

