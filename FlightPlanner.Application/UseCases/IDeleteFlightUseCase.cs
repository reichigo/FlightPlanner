namespace FlightPlanner.Application.UseCases;

public interface IDeleteFlightUseCase
{
    Task<bool> ExecuteAsync(Guid id, CancellationToken ct);
}

