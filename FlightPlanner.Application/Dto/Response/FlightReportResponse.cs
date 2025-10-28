namespace FlightPlanner.Application.Dto.Response;

public sealed record FlightReportResponse(
    List<FlightResponse> Flights,
    int TotalFlights,
    double TotalDistanceKm,
    double TotalFuelKg
);

