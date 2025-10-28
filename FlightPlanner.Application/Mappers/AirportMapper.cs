using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Domain.Entities;

namespace FlightPlanner.Application.Mappers;

public static class AirportMapper
{
    public static Airport ToDomain(this CreateAirportRequest request)
    {
        var iataCode = new IataCode(request.Iata);
        var location = new GeoCoordinate(request.Latitude, request.Longitude);
        return new Airport(iataCode, request.Name, location);
    }

    public static AirportResponse ToResponse(this Airport airport)
    {
        return new AirportResponse(
            airport.Id,
            airport.Iata.Code,
            airport.Name,
            airport.Location.Latitude,
            airport.Location.Longitude
        );
    }
}