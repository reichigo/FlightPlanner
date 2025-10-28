using FlightPlanner.Infrastructure.DataSources.Mongo.Models;

namespace FlightPlanner.Infrastructure.DataSources.Mongo;

public interface IAirportMongoDataSource : IMongoDataSource<AirportMongoModel, Guid>
{
    Task<AirportMongoModel?> GetByIataAsync(string iata);
    Task<IEnumerable<AirportMongoModel>> SearchAsync(string searchTerm, int skip, int take);
    Task<int> CountAsync(string? searchTerm = null);
}

