namespace FlightPlanner.Domain.Entities.Interfaces;

public interface IFuelEstimator
{
    FlightEstimates Estimate(Aircraft aircraft, double distanceKm);
}