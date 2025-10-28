using System.Linq.Expressions;

namespace FlightPlanner.Infrastructure.DataSources.Mongo;

public interface IMongoDataSource<TModelMongo, TId>
    where TModelMongo : class where TId : struct
{
    Task CreateAsync(TModelMongo modelMongo);
    Task<TModelMongo?> GetByIdAsync(TId id);
    Task<IEnumerable<TModelMongo>> GetByIdsAsync(IEnumerable<TId> ids);
    Task<IEnumerable<TModelMongo>> GetAllAsync();
    Task UpdateAsync(TModelMongo modelMongo);
    Task<IEnumerable<TModelMongo>> GetPaginatedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<TModelMongo, bool>> filter);
    Task DeleteAsync(TId id);
    Task AddToArrayAsync<TArrayItem>(
        TId id,
        string arrayFieldName,
        IEnumerable<TArrayItem> arrayItems);
}