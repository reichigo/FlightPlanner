using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.Mappers;
using FlightPlanner.Domain.Entities.Interfaces;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class GetFlightUseCase(
    IFlightRepository flightRepository,
    IAirportRepository airportRepository,
    IAircraftRepository aircraftRepository,
    IDistanceCalculator distanceCalculator,
    IFuelEstimator fuelEstimator) : IGetFlightUseCase
{
    public async Task<FlightResponse?> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var flight = await flightRepository.GetByIdAsync(id, ct);
        if (flight == null) return null;

        var origin = await airportRepository.GetByIdAsync(flight.OriginId, ct);
        var destination = await airportRepository.GetByIdAsync(flight.DestinationId, ct);
        var aircraft = await aircraftRepository.GetByIdAsync(flight.AircraftId, ct);

        if (origin == null || destination == null || aircraft == null) return null;

        var estimates = flight.Plan(distanceCalculator, fuelEstimator, origin, destination, aircraft);

        return flight.ToResponse(origin, destination, aircraft, estimates);
    }
}

