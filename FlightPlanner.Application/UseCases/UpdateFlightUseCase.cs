using System.Net;
using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.Mappers;
using FlightPlanner.Domain.Entities.Interfaces;
using FlightPlanner.Domain.Exceptions;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class UpdateFlightUseCase(
    IFlightRepository flightRepository,
    IAirportRepository airportRepository,
    IAircraftRepository aircraftRepository,
    IDistanceCalculator distanceCalculator,
    IFuelEstimator fuelEstimator) : IUpdateFlightUseCase
{
    public async Task<FlightResponse?> ExecuteAsync(Guid id, UpdateFlightRequest request, CancellationToken ct)
    {
        var existingFlight = await flightRepository.GetByIdAsync(id, ct);
        if (existingFlight == null) return null;

        var origin = await airportRepository.GetByIdAsync(request.OriginAirportId, ct);
        if (origin == null) throw new GlobalExceptions(HttpStatusCode.NotFound, "Origin airport not found");

        var destination = await airportRepository.GetByIdAsync(request.DestinationAirportId, ct);
        if (destination == null) throw new GlobalExceptions(HttpStatusCode.NotFound, "Destination airport not found");

        var aircraft = await aircraftRepository.GetByIdAsync(request.AircraftId, ct);
        if (aircraft == null) throw new GlobalExceptions(HttpStatusCode.NotFound, "Aircraft not found");

        // Note: UpdateFlightRequest doesn't have ToDomain mapper because we need to preserve the existing flight's ID
        // This is a limitation of the current Flight entity design
        var updatedFlight = new Domain.Entities.Flight(
            request.OriginAirportId,
            request.DestinationAirportId,
            request.AircraftId,
            request.DepartureAt
        );

        var estimates = updatedFlight.Plan(distanceCalculator, fuelEstimator, origin, destination, aircraft);
        var savedFlight = await flightRepository.UpdateAsync(updatedFlight, ct);
        if (savedFlight == null) return null;

        return savedFlight.ToResponse(origin, destination, aircraft, estimates);
    }
}

