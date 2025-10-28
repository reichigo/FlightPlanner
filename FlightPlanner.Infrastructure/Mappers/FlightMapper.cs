using FlightPlanner.Domain.Entities;
using FlightPlanner.Infrastructure.DataSources.Mongo.Models;

namespace FlightPlanner.Infrastructure.Mappers;

internal static class FlightMapper
{
    public static Flight ToDomain(this FlightMongoModel flight)
    {
        var flightEntity = new Flight(
            flight.OriginId,
            flight.DestinationId,
            flight.AircraftId,
            flight.DepartureAt
        );
        
        // Set the Id using reflection since it's get-only
        var backingField = typeof(Flight).GetField("<Id>k__BackingField", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        backingField?.SetValue(flightEntity, flight.Id);
        
        return flightEntity;
    }

    public static FlightMongoModel ToMongo(this Flight flight)
    {
        return new FlightMongoModel
        {
            Id = flight.Id,
            OriginId = flight.OriginId,
            DestinationId = flight.DestinationId,
            AircraftId = flight.AircraftId,
            DepartureAt = flight.DepartureAt
        };
    }
}

