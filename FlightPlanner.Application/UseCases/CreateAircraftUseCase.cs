using System.Net;
using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.Mappers;
using FlightPlanner.Domain.Exceptions;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class CreateAircraftUseCase(IAircraftRepository aircraftRepository) : ICreateAircraftUseCase
{
    public async Task<AircraftResponse> ExecuteAsync(CreateAircraftRequest request, CancellationToken ct)
    {
        // Check if aircraft with same model already exists
        var existing = await aircraftRepository.GetByModelAsync(request.Model, ct);
        if (existing != null)
        {
            throw new GlobalExceptions(HttpStatusCode.Conflict, $"Aircraft with model '{request.Model}' already exists");
        }

        var aircraft = request.ToDomain();
        var saved = await aircraftRepository.AddAsync(aircraft, ct);

        return saved.ToResponse();
    }
}

