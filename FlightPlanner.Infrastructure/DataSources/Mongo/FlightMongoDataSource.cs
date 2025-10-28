using FlightPlanner.Infrastructure.DataSources.Mongo.Models;
using MongoDB.Driver;

namespace FlightPlanner.Infrastructure.DataSources.Mongo;

public class FlightMongoDataSource(IMongoCollection<FlightMongoModel> collection)
    : MongoDataSource<FlightMongoModel, Guid>(collection),
        IFlightMongoDataSource;

