using FlightPlanner.Infrastructure.DataSources.Mongo.Models;

namespace FlightPlanner.Infrastructure.DataSources.Mongo;

public interface IAircraftMongoDataSource : IMongoDataSource<AircraftMongoModel, Guid>
{
    Task<AircraftMongoModel?> GetByModelAsync(string model);
}