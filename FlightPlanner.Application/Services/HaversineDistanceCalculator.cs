using FlightPlanner.Domain.Entities;
using FlightPlanner.Domain.Entities.Interfaces;

namespace FlightPlanner.Application.Services;

public class HaversineDistanceCalculator : IDistanceCalculator
{
    private const double EarthRadiusKm = 6371.0;

    public double DistanceKm(GeoCoordinate a, GeoCoordinate b)
    {
        var lat1Rad = DegreesToRadians(a.Latitude);
        var lat2Rad = DegreesToRadians(b.Latitude);
        var deltaLat = DegreesToRadians(b.Latitude - a.Latitude);
        var deltaLon = DegreesToRadians(b.Longitude - a.Longitude);

        var haversine = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        var centralAngle = 2 * Math.Atan2(Math.Sqrt(haversine), Math.Sqrt(1 - haversine));

        return EarthRadiusKm * centralAngle;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
}

