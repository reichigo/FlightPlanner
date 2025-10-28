using FlightPlanner.Domain.Entities;

namespace FlightPlanner.Domain.Repositories;

public interface IAirportRepository
{
    Task<Airport?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Airport>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<Airport?> GetByIataAsync(string iata, CancellationToken ct = default);
    Task<List<Airport>> GetAllAsync(CancellationToken ct = default);
    Task<List<Airport>> SearchAsync(string searchTerm, int skip, int take, CancellationToken ct = default);
    Task<int> CountAsync(string? searchTerm = null, CancellationToken ct = default);
    Task<Airport> AddAsync(Airport airport, CancellationToken ct = default);
    Task<Airport?> UpdateAsync(Airport airport, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}

