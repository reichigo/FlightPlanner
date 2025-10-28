using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.Mappers;
using FlightPlanner.Domain.Entities.Interfaces;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class GetAllFlightsUseCase(
    IFlightRepository flightRepository,
    IAirportRepository airportRepository,
    IAircraftRepository aircraftRepository,
    IDistanceCalculator distanceCalculator,
    IFuelEstimator fuelEstimator) : IGetAllFlightsUseCase
{
    public async Task<List<FlightResponse>> ExecuteAsync(CancellationToken ct)
    {
        var flights = await flightRepository.GetAllAsync(ct);

        // Coletar todos os IDs únicos de aeroportos e aeronaves
        var airportIds = flights
            .SelectMany(f => new[] { f.OriginId, f.DestinationId })
            .Distinct()
            .ToList();

        var aircraftIds = flights
            .Select(f => f.AircraftId)
            .Distinct()
            .ToList();

        var airports = await airportRepository.GetByIdsAsync(airportIds, ct);
        var aircrafts = await aircraftRepository.GetByIdsAsync(aircraftIds, ct);

        var responses = new List<FlightResponse>();

        foreach (var flight in flights)
        {
            var origin = airports.FirstOrDefault(x => x.Id == flight.OriginId);
            var destination = airports.FirstOrDefault(x => x.Id == flight.DestinationId);
            var aircraft = aircrafts.FirstOrDefault(x => x.Id ==flight.AircraftId);

            if (origin == null || destination == null || aircraft == null) continue;

            var estimates = flight.Plan(distanceCalculator, fuelEstimator, origin, destination, aircraft);
            responses.Add(flight.ToResponse(origin, destination, aircraft, estimates));
        }

        return responses;
    }
}

