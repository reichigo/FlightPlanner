using System.Net;
using FlightPlanner.Domain.Entities.Interfaces;
using FlightPlanner.Domain.Exceptions;

namespace FlightPlanner.Domain.Entities;

public class Flight(Guid originId, Guid destinationId, Guid aircraftId, DateTime? departureAt = null)
{
    public Guid Id { get; } = Guid.NewGuid();

    public Guid OriginId { get; } = originId == destinationId
        ? throw new GlobalExceptions(HttpStatusCode.BadRequest, "Origin and destination must differ.")
        : originId;
    public Guid DestinationId { get; } = destinationId;
    public Guid AircraftId { get; } = aircraftId;
    public DateTime? DepartureAt { get; } = departureAt;

    public FlightEstimates Plan(
        IDistanceCalculator distance,
        IFuelEstimator fuel,
        Airport origin,
        Airport destination,
        Aircraft aircraft)
    {
        if (origin.Id != OriginId || destination.Id != DestinationId || aircraft.Id != AircraftId)
            throw new GlobalExceptions(HttpStatusCode.BadRequest, "Mismatched aggregates.");

        var dKm = distance.DistanceKm(origin.Location, destination.Location);
        return fuel.Estimate(aircraft, dKm);
    }
}