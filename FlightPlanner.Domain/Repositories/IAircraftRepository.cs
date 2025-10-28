using FlightPlanner.Domain.Entities;

namespace FlightPlanner.Domain.Repositories;

public interface IAircraftRepository
{
    Task<Aircraft?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Aircraft>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<Aircraft?> GetByModelAsync(string model, CancellationToken ct = default);
    Task<List<Aircraft>> GetAllAsync(CancellationToken ct = default);
    Task<Aircraft> AddAsync(Aircraft aircraft, CancellationToken ct = default);
    Task<Aircraft?> UpdateAsync(Aircraft aircraft, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

