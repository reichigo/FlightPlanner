using FlightPlanner.Domain.Entities;
using FlightPlanner.Domain.Repositories;
using FlightPlanner.Infrastructure.DataSources.Mongo;
using FlightPlanner.Infrastructure.Mappers;

namespace FlightPlanner.Infrastructure.Repositories;

public class AirportRepository(IAirportMongoDataSource dataSource) : IAirportRepository
{
    public Task<Airport?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return dataSource.GetByIdAsync(id)
            .ContinueWith(t => t.Result?.ToDomain(), ct);
    }

    public async Task<List<Airport>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var models = await dataSource.GetByIdsAsync(ids);
        return models.Select(m => m.ToDomain()).ToList();
    }

    public Task<Airport?> GetByIataAsync(string iata, CancellationToken ct = default)
    {
        return dataSource.GetByIataAsync(iata)
            .ContinueWith(t => t.Result?.ToDomain(), ct);
    }

    public async Task<List<Airport>> GetAllAsync(CancellationToken ct = default)
    {
        var models = await dataSource.GetAllAsync();
        return models.Select(m => m.ToDomain()).ToList();
    }

    public async Task<List<Airport>> SearchAsync(string searchTerm, int skip, int take, CancellationToken ct = default)
    {
        var models = await dataSource.SearchAsync(searchTerm, skip, take);
        return models.Select(m => m.ToDomain()).ToList();
    }

    public Task<int> CountAsync(string? searchTerm = null, CancellationToken ct = default)
    {
        return dataSource.CountAsync(searchTerm);
    }

    public Task<Airport> AddAsync(Airport airport, CancellationToken ct = default)
    {
        var model = airport.ToMongo();
        return dataSource.CreateAsync(model)
            .ContinueWith(_ => airport, ct);
    }

    public Task<Airport?> UpdateAsync(Airport airport, CancellationToken ct = default)
    {
        var model = airport.ToMongo();
        return dataSource.UpdateAsync(model)
            .ContinueWith(_ => (Airport?)airport, ct);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return dataSource.DeleteAsync(id)
            .ContinueWith(_ => true, ct);
    }
}

