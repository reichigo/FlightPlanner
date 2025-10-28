using System.Net;
using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.Mappers;
using FlightPlanner.Domain.Exceptions;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class CreateAirportUseCase(IAirportRepository airportRepository) : ICreateAirportUseCase
{
    public async Task<AirportResponse> ExecuteAsync(CreateAirportRequest request, CancellationToken ct)
    {
        // Check if airport with same IATA already exists
        var existing = await airportRepository.GetByIataAsync(request.Iata, ct);
        if (existing != null)
        {
            throw new GlobalExceptions(HttpStatusCode.Conflict, $"Airport with IATA code '{request.Iata}' already exists");
        }

        var airport = request.ToDomain();
        var saved = await airportRepository.AddAsync(airport, ct);

        return saved.ToResponse();
    }
}