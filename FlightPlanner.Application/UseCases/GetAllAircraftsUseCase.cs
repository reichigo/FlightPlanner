using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.Mappers;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class GetAllAircraftsUseCase(IAircraftRepository aircraftRepository) : IGetAllAircraftsUseCase
{
    public async Task<List<AircraftResponse>> ExecuteAsync(CancellationToken ct)
    {
        var aircrafts = await aircraftRepository.GetAllAsync(ct);
        return aircrafts.Select(a => a.ToResponse()).ToList();
    }
}

