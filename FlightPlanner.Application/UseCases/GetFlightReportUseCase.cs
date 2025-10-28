using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.Mappers;
using FlightPlanner.Domain.Entities.Interfaces;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class GetFlightReportUseCase(
    IFlightRepository flightRepository,
    IAirportRepository airportRepository,
    IAircraftRepository aircraftRepository,
    IDistanceCalculator distanceCalculator,
    IFuelEstimator fuelEstimator) : IGetFlightReportUseCase
{
    public async Task<FlightReportResponse> ExecuteAsync(CancellationToken ct)
    {
        var flights = await flightRepository.GetAllAsync(ct);
        var flightResponses = new List<FlightResponse>();
        double totalDistance = 0;
        double totalFuel = 0;

        foreach (var flight in flights)
        {
            var origin = await airportRepository.GetByIdAsync(flight.OriginId, ct);
            var destination = await airportRepository.GetByIdAsync(flight.DestinationId, ct);
            var aircraft = await aircraftRepository.GetByIdAsync(flight.AircraftId, ct);

            if (origin == null || destination == null || aircraft == null) continue;

            var estimates = flight.Plan(distanceCalculator, fuelEstimator, origin, destination, aircraft);

            totalDistance += estimates.DistanceKm;
            totalFuel += estimates.FuelKg;

            flightResponses.Add(flight.ToResponse(origin, destination, aircraft, estimates));
        }

        return new FlightReportResponse(
            flightResponses,
            flightResponses.Count,
            totalDistance,
            totalFuel
        );
    }
}

