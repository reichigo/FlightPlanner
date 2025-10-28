using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.Mappers;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class GetAircraftUseCase(IAircraftRepository aircraftRepository) : IGetAircraftUseCase
{
    public async Task<AircraftResponse?> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var aircraft = await aircraftRepository.GetByIdAsync(id, ct);
        return aircraft?.ToResponse();
    }
}

