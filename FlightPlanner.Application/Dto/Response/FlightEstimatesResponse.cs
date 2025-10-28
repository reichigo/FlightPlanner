namespace FlightPlanner.Application.Dto.Response;

public sealed record FlightEstimatesResponse(
    double DistanceKm,
    TimeSpan Duration,
    double FuelKg
);

