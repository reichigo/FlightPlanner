using FlightPlanner.Domain.Entities;
using FlightPlanner.Domain.Repositories;
using FlightPlanner.Infrastructure.DataSources.Mongo;
using FlightPlanner.Infrastructure.Mappers;

namespace FlightPlanner.Infrastructure.Repositories;

public class AircraftRepository(IAircraftMongoDataSource dataSource) : IAircraftRepository
{
    public Task<Aircraft?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return dataSource.GetByIdAsync(id)
            .ContinueWith(t => t.Result?.ToDomain(), ct);
    }

    public async Task<List<Aircraft>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var models = await dataSource.GetByIdsAsync(ids);
        return models.Select(m => m.ToDomain()).ToList();
    }

    public Task<Aircraft?> GetByModelAsync(string model, CancellationToken ct = default)
    {
        return dataSource.GetByModelAsync(model)
            .ContinueWith(t => t.Result?.ToDomain(), ct);
    }

    public async Task<List<Aircraft>> GetAllAsync(CancellationToken ct = default)
    {
        var models = await dataSource.GetAllAsync();
        return models.Select(m => m.ToDomain()).ToList();
    }

    public Task<Aircraft> AddAsync(Aircraft aircraft, CancellationToken ct = default)
    {
        var model = aircraft.ToMongo();
        return dataSource.CreateAsync(model)
            .ContinueWith(_ => aircraft, ct);
    }

    public Task<Aircraft?> UpdateAsync(Aircraft aircraft, CancellationToken ct = default)
    {
        var model = aircraft.ToMongo();
        return dataSource.UpdateAsync(model)
            .ContinueWith(_ => (Aircraft?)aircraft, ct);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return dataSource.DeleteAsync(id)
            .ContinueWith(_ => true, ct);
    }
}

