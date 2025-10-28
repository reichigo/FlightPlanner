using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Application.Mappers;
using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class GetAirportUseCase(IAirportRepository airportRepository) : IGetAirportUseCase
{
    public async Task<AirportResponse?> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var airport = await airportRepository.GetByIdAsync(id, ct);

        return airport?.ToResponse();
    }
}

