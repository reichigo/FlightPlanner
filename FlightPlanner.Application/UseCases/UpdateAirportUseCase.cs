using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.Mappers;
using FlightPlanner.Domain.Entities;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class UpdateAirportUseCase(IAirportRepository airportRepository) : IUpdateAirportUseCase
{
    public async Task<AirportResponse?> ExecuteAsync(Guid id, UpdateAirportRequest request, CancellationToken ct)
    {
        var existing = await airportRepository.GetByIdAsync(id, ct);
        if (existing == null) return null;

        var iataCode = new IataCode(request.Iata);
        var location = new GeoCoordinate(request.Latitude, request.Longitude);

        existing.Update(iataCode, request.Name, location);

        var updated = await airportRepository.UpdateAsync(existing, ct);
        if (updated == null) return null;

        return updated.ToResponse();
    }
}

