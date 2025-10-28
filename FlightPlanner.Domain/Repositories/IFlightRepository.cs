using FlightPlanner.Domain.Entities;

namespace FlightPlanner.Domain.Repositories;

public interface IFlightRepository
{
    Task<Flight?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<Flight>> GetAllAsync(CancellationToken ct = default);
    Task<Flight> AddAsync(Flight flight, CancellationToken ct = default);
    Task<Flight?> UpdateAsync(Flight flight, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}