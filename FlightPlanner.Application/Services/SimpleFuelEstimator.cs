using FlightPlanner.Domain.Entities;
using FlightPlanner.Domain.Entities.Interfaces;

namespace FlightPlanner.Application.Services;

public class SimpleFuelEstimator : IFuelEstimator
{
    public FlightEstimates Estimate(Aircraft aircraft, double distanceKm)
    {
        // Calculate flight time based on cruise speed
        var flightTimeHours = distanceKm / (aircraft.CruiseSpeedKts * 1.852); // Convert knots to km/h
        var duration = TimeSpan.FromHours(flightTimeHours);

        // Calculate fuel: (flight time * burn rate) + takeoff fuel
        var fuelKg = (flightTimeHours * aircraft.FuelBurnPerHourKg) + aircraft.TakeoffFuelKg;

        return new FlightEstimates(distanceKm, duration, fuelKg);
    }
}

