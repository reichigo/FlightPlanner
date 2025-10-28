using FlightPlanner.Infrastructure.DataSources.Mongo.Models;
using MongoDB.Driver;

namespace FlightPlanner.Infrastructure.DataSources.Mongo;

public class AircraftMongoDataSource(IMongoCollection<AircraftMongoModel> collection)
    : MongoDataSource<AircraftMongoModel, Guid>(collection),
        IAircraftMongoDataSource
{
    public async Task<AircraftMongoModel?> GetByModelAsync(string model)
    {
        return await _collection.Find(a => a.Model == model).FirstOrDefaultAsync();
    }
}

