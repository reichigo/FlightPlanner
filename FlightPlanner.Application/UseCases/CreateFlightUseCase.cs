using System.Net;
using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.Mappers;
using FlightPlanner.Domain.Entities.Interfaces;
using FlightPlanner.Domain.Exceptions;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class CreateFlightUseCase(
    IFlightRepository flightRepository,
    IAirportRepository airportRepository,
    IAircraftRepository aircraftRepository,
    IDistanceCalculator distanceCalculator,
    IFuelEstimator fuelEstimator) : ICreateFlightUseCase
{
    public async Task<FlightResponse> ExecuteAsync(CreateFlightRequest request, CancellationToken ct)
    {
        var origin = await airportRepository.GetByIdAsync(request.OriginAirportId, ct);
        if (origin == null) throw new GlobalExceptions(HttpStatusCode.NotFound, "Origin airport not found");

        var destination = await airportRepository.GetByIdAsync(request.DestinationAirportId, ct);
        if (destination == null) throw new GlobalExceptions(HttpStatusCode.NotFound, "Destination airport not found");

        var aircraft = await aircraftRepository.GetByIdAsync(request.AircraftId, ct);
        if (aircraft == null) throw new GlobalExceptions(HttpStatusCode.NotFound, "Aircraft not found");

        var flight = request.ToDomain();
        var estimates = flight.Plan(distanceCalculator, fuelEstimator, origin, destination, aircraft);
        var savedFlight = await flightRepository.AddAsync(flight, ct);

        return savedFlight.ToResponse(origin, destination, aircraft, estimates);
    }
}

