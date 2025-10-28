using FlightPlanner.Infrastructure.DataSources.Mongo.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FlightPlanner.Infrastructure.DataSources.Mongo;

public class AirportMongoDataSource(IMongoCollection<AirportMongoModel> collection)
    : MongoDataSource<AirportMongoModel, Guid>(collection),
        IAirportMongoDataSource
{
    public async Task<AirportMongoModel?> GetByIataAsync(string iata)
    {
        return await _collection.Find(a => a.Iata == iata.ToUpperInvariant()).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<AirportMongoModel>> SearchAsync(string searchTerm, int skip, int take)
    {
        var filter = string.IsNullOrWhiteSpace(searchTerm)
            ? Builders<AirportMongoModel>.Filter.Empty
            : Builders<AirportMongoModel>.Filter.Or(
                Builders<AirportMongoModel>.Filter.Regex(a => a.Name, new BsonRegularExpression(searchTerm, "i")),
                Builders<AirportMongoModel>.Filter.Regex(a => a.Iata, new BsonRegularExpression(searchTerm, "i"))
            );

        return await _collection.Find(filter).Skip(skip).Limit(take).ToListAsync();
    }

    public async Task<int> CountAsync(string? searchTerm = null)
    {
        var filter = string.IsNullOrWhiteSpace(searchTerm)
            ? Builders<AirportMongoModel>.Filter.Empty
            : Builders<AirportMongoModel>.Filter.Or(
                Builders<AirportMongoModel>.Filter.Regex(a => a.Name, new BsonRegularExpression(searchTerm, "i")),
                Builders<AirportMongoModel>.Filter.Regex(a => a.Iata, new BsonRegularExpression(searchTerm, "i"))
            );

        return (int)await _collection.CountDocumentsAsync(filter);
    }
}

