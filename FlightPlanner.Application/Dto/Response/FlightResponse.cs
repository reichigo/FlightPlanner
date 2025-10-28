namespace FlightPlanner.Application.Dto.Response;

public sealed record FlightResponse(
    Guid Id,
    AirportResponse Origin,
    AirportResponse Destination,
    AircraftResponse Aircraft,
    DateTime? DepartureAt,
    FlightEstimatesResponse? Estimates
);

