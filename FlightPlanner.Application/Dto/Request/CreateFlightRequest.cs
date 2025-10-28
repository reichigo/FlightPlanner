using System.Text.Json.Serialization;

namespace FlightPlanner.Application.Dto.Request;

public sealed record CreateFlightRequest(
    [property: JsonPropertyName("originAirportId")] Guid OriginAirportId,
    [property: JsonPropertyName("destinationAirportId")] Guid DestinationAirportId,
    [property: JsonPropertyName("aircraftId")] Guid AircraftId,
    [property: JsonPropertyName("departureAt")] DateTime? DepartureAt
);

