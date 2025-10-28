using FlightPlanner.Domain.Entities;
using FlightPlanner.Infrastructure.DataSources.Mongo.Models;

namespace FlightPlanner.Infrastructure.Mappers;

internal static class AirportMapper
{
    public static Airport ToDomain(this AirportMongoModel airport)
    {
        var iata = new IataCode(airport.Iata);
        var location = new GeoCoordinate(airport.Latitude, airport.Longitude);
        var airportEntity = new Airport(iata, airport.Name, location);
        
        // Set the Id using reflection since it's private set
        var idProperty = typeof(Airport).GetProperty("Id");
        idProperty?.SetValue(airportEntity, airport.Id);
        
        return airportEntity;
    }

    public static AirportMongoModel ToMongo(this Airport airport)
    {
        return new AirportMongoModel
        {
            Id = airport.Id,
            Iata = airport.Iata.Code,
            Name = airport.Name,
            Latitude = airport.Location.Latitude,
            Longitude = airport.Location.Longitude
        };
    }
}

