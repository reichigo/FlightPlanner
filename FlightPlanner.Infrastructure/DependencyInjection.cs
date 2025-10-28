using FlightPlanner.Domain.Repositories;
using FlightPlanner.Infrastructure;
using FlightPlanner.Infrastructure.DataSources.Mongo;
using FlightPlanner.Infrastructure.DataSources.Mongo.Models;
using FlightPlanner.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FlightPlanner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB Settings
        services.Configure<MongoDbSettings>(options =>
        {
            configuration.GetSection("MongoDbSettings").Bind(options);
        });

        // MongoDB Client with Standard GUID representation
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
            clientSettings.GuidRepresentation = GuidRepresentation.Standard;
            return new MongoClient(clientSettings);
        });

        // MongoDB Collections
        services.AddSingleton<IMongoCollection<AirportMongoModel>>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            var database = client.GetDatabase(settings.DatabaseName);
            return database.GetCollection<AirportMongoModel>(settings.AirportsCollectionName);
        });

        services.AddSingleton<IMongoCollection<AircraftMongoModel>>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            var database = client.GetDatabase(settings.DatabaseName);
            return database.GetCollection<AircraftMongoModel>(settings.AircraftsCollectionName);
        });

        services.AddSingleton<IMongoCollection<FlightMongoModel>>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            var database = client.GetDatabase(settings.DatabaseName);
            return database.GetCollection<FlightMongoModel>(settings.FlightsCollectionName);
        });

        // DataSources
        services.AddScoped<IAirportMongoDataSource, AirportMongoDataSource>();
        services.AddScoped<IAircraftMongoDataSource, AircraftMongoDataSource>();
        services.AddScoped<IFlightMongoDataSource, FlightMongoDataSource>();

        // Repositories
        services.AddScoped<IAirportRepository, AirportRepository>();
        services.AddScoped<IAircraftRepository, AircraftRepository>();
        services.AddScoped<IFlightRepository, FlightRepository>();

        return services;
    }
}

