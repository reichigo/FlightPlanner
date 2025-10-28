using FlightPlanner.Domain.Repositories;

namespace FlightPlanner.Application.UseCases;

public class DeleteFlightUseCase(IFlightRepository flightRepository) : IDeleteFlightUseCase
{
    public async Task<bool> ExecuteAsync(Guid id, CancellationToken ct)
    {
        return await flightRepository.DeleteAsync(id, ct);
    }
}

