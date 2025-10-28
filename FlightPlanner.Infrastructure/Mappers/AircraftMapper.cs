using FlightPlanner.Domain.Entities;
using FlightPlanner.Infrastructure.DataSources.Mongo.Models;

namespace FlightPlanner.Infrastructure.Mappers;

internal static class AircraftMapper
{
    public static Aircraft ToDomain(this AircraftMongoModel aircraft)
    {
        var aircraftEntity = new Aircraft(
            aircraft.Model,
            aircraft.CruiseSpeedKts,
            aircraft.FuelBurnPerHourKg,
            aircraft.TakeoffFuelKg
        );

        // Set the Id using reflection since it's private set
        var idProperty = typeof(Aircraft).GetProperty("Id");
        idProperty?.SetValue(aircraftEntity, aircraft.Id);

        return aircraftEntity;
    }

    public static AircraftMongoModel ToMongo(this Aircraft aircraft)
    {
        return new AircraftMongoModel
        {
            Id = aircraft.Id,
            Model = aircraft.Model,
            CruiseSpeedKts = aircraft.CruiseSpeedKts,
            FuelBurnPerHourKg = aircraft.FuelBurnPerHourKg,
            TakeoffFuelKg = aircraft.TakeoffFuelKg
        };
    }
}