using System.Net;
using FlightPlanner.Domain.Exceptions;

namespace FlightPlanner.Domain.Entities;

public class Aircraft(string model, double cruiseSpeedKts, double fuelBurnPerHourKg, double takeoffFuelKg)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Model { get; private set; } = string.IsNullOrWhiteSpace(model) ? throw new GlobalExceptions(HttpStatusCode.BadRequest, "Model is required.") : model;
    public double CruiseSpeedKts { get; private set; } = cruiseSpeedKts > 0 ? cruiseSpeedKts : throw new GlobalExceptions(HttpStatusCode.BadRequest, "Cruise speed must be positive.");
    public double FuelBurnPerHourKg { get; private set; } = fuelBurnPerHourKg > 0 ? fuelBurnPerHourKg : throw new GlobalExceptions(HttpStatusCode.BadRequest, "Fuel burn must be positive.");
    public double TakeoffFuelKg { get; private set; } = takeoffFuelKg >= 0 ? takeoffFuelKg : throw new GlobalExceptions(HttpStatusCode.BadRequest, "Takeoff fuel must be non-negative.");
}