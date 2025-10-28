using FlightPlanner.Infrastructure.DataSources.Mongo.Models;

namespace FlightPlanner.Infrastructure.DataSources.Mongo;

public interface IFlightMongoDataSource : IMongoDataSource<FlightMongoModel, Guid>;

