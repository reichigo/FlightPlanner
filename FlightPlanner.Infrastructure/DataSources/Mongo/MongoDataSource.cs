using System.Linq.Expressions;
using FlightPlanner.Domain.Exceptions;
using MongoDB.Driver;

namespace FlightPlanner.Infrastructure.DataSources.Mongo;

public abstract class MongoDataSource<TModelMongo, TId>(IMongoCollection<TModelMongo> collection) : IMongoDataSource<TModelMongo, TId>
    where TModelMongo : class where TId : struct
{
    public IMongoCollection<TModelMongo> _collection = collection;

    public Task CreateAsync(TModelMongo modelMongo)
    {
        return _collection.InsertOneAsync(modelMongo);
    }

    public async Task<TModelMongo?> GetByIdAsync(TId id)
    {
        return await _collection.Find(Builders<TModelMongo>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<TModelMongo>> GetByIdsAsync(IEnumerable<TId> ids)
    {
        var filter = Builders<TModelMongo>.Filter.In("_id", ids);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<TModelMongo>> GetAllAsync()
    {
        return await _collection.Find(Builders<TModelMongo>.Filter.Empty).ToListAsync();
    }

    public async Task UpdateAsync(TModelMongo modelMongo)
    {
        var idProperty = typeof(TModelMongo).GetProperty("Id");
        if (idProperty != null)
        {
            var id = idProperty.GetValue(modelMongo, null);
            if (id != null)
            {
                await _collection.ReplaceOneAsync(
                    Builders<TModelMongo>.Filter.Eq("_id", id),
                    modelMongo);
            }
        }
    }

    public async Task<IEnumerable<TModelMongo>> GetPaginatedAsync(int pageNumber, int pageSize,
        Expression<Func<TModelMongo, bool>> filter)
    {
        if (pageNumber <= 0)
        {
            throw new GlobalExceptions(System.Net.HttpStatusCode.BadRequest, "Page number must be greater than zero.");
        }

        if (pageSize <= 0)
        {
            throw new GlobalExceptions(System.Net.HttpStatusCode.BadRequest, "Page size must be greater than zero.");
        }

        var filteredQuery = _collection.Find(filter)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize);

        return await filteredQuery.ToListAsync();
    }
    
    public Task DeleteAsync(TId id)
    {
        return _collection.DeleteOneAsync(Builders<TModelMongo>.Filter.Eq("_id", id));
    }

    public Task AddToArrayAsync<TArrayItem>(TId id, string arrayFieldName, IEnumerable<TArrayItem> arrayItems)
    {
        var update = Builders<TModelMongo>.Update.PushEach(arrayFieldName, arrayItems);
        return _collection.UpdateOneAsync(Builders<TModelMongo>.Filter.Eq("_id", id), update);
    }
}