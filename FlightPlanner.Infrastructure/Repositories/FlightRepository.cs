using FlightPlanner.Domain.Entities;
using FlightPlanner.Domain.Repositories;
using FlightPlanner.Infrastructure.DataSources.Mongo;
using FlightPlanner.Infrastructure.Mappers;

namespace FlightPlanner.Infrastructure.Repositories;

public class FlightRepository(IFlightMongoDataSource dataSource) : IFlightRepository
{
    public Task<Flight?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return dataSource.GetByIdAsync(id)
            .ContinueWith(t => t.Result?.ToDomain(), ct);
    }

    public async Task<List<Flight>> GetAllAsync(CancellationToken ct = default)
    {
        var models = await dataSource.GetAllAsync();
        return models.Select(m => m.ToDomain()).ToList();
    }

    public Task<Flight> AddAsync(Flight flight, CancellationToken ct = default)
    {
        var model = flight.ToMongo();
        return dataSource.CreateAsync(model)
            .ContinueWith(_ => flight, ct);
    }

    public Task<Flight?> UpdateAsync(Flight flight, CancellationToken ct = default)
    {
        var model = flight.ToMongo();
        return dataSource.UpdateAsync(model)
            .ContinueWith(_ => (Flight?)flight, ct);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return dataSource.DeleteAsync(id)
            .ContinueWith(_ => true, ct);
    }
}

