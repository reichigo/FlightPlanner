namespace FlightPlanner.Application.Dto.Request;

public sealed record UpdateFlightRequest(
    Guid OriginAirportId,
    Guid DestinationAirportId,
    Guid AircraftId,
    DateTime? DepartureAt
);

