using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Domain.Entities;

namespace FlightPlanner.Application.Mappers;

public static class FlightMapper
{
    public static Flight ToDomain(this CreateFlightRequest request)
    {
        return new Flight(
            request.OriginAirportId,
            request.DestinationAirportId,
            request.AircraftId,
            request.DepartureAt
        );
    }

    public static FlightResponse ToResponse(
        this Flight flight,
        Airport origin,
        Airport destination,
        Aircraft aircraft,
        FlightEstimates estimates)
    {
        return new FlightResponse(
            flight.Id,
            origin.ToResponse(),
            destination.ToResponse(),
            aircraft.ToResponse(),
            flight.DepartureAt,
            new FlightEstimatesResponse(estimates.DistanceKm, estimates.Duration, estimates.FuelKg)
        );
    }
}

