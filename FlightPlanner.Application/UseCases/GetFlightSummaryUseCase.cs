using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Domain.Entities.Interfaces;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class GetFlightSummaryUseCase(
    IFlightRepository flightRepository,
    IAirportRepository airportRepository,
    IAircraftRepository aircraftRepository,
    IDistanceCalculator distanceCalculator,
    IFuelEstimator fuelEstimator) : IGetFlightSummaryUseCase
{
    public async Task<FlightSummaryResponse> ExecuteAsync(CancellationToken ct)
    {
        var flights = await flightRepository.GetAllAsync(ct);
        var airportIds = flights
            .SelectMany(f => new[] { f.OriginId, f.DestinationId })
            .Distinct()
            .ToList();

        var airports = await airportRepository.GetByIdsAsync(airportIds, ct);
        double totalDistance = 0;
        double totalFuel = 0;
        int validFlights = 0;

        foreach (var flight in flights)
        {
            var origin = airports.FirstOrDefault(x => x.Id == flight.OriginId);
            var destination = airports.FirstOrDefault(x => x.Id ==flight.DestinationId);
            var aircraft = await aircraftRepository.GetByIdAsync(flight.AircraftId, ct);

            if (origin == null || destination == null || aircraft == null) continue;

            var estimates = flight.Plan(distanceCalculator, fuelEstimator, origin, destination, aircraft);

            totalDistance += estimates.DistanceKm;
            totalFuel += estimates.FuelKg;
            validFlights++;
        }

        var avgDistance = validFlights > 0 ? totalDistance / validFlights : 0;
        var avgFuel = validFlights > 0 ? totalFuel / validFlights : 0;

        return new FlightSummaryResponse(
            validFlights,
            totalDistance,
            totalFuel,
            avgDistance,
            avgFuel
        );
    }
}

