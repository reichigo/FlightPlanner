namespace FlightPlanner.Domain.Entities.Interfaces;

public interface IDistanceCalculator
{
    double DistanceKm(GeoCoordinate a, GeoCoordinate b);
}