namespace FlightPlanner.Application.Dto.Response;

public sealed record FlightSummaryResponse(
    int TotalFlights,
    double TotalDistanceKm,
    double TotalFuelKg,
    double AverageDistanceKm,
    double AverageFuelKg
);

