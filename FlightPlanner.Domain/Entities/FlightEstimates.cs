using System.Net;
using FlightPlanner.Domain.Exceptions;

namespace FlightPlanner.Domain.Entities;

public sealed record FlightEstimates(double DistanceKm, TimeSpan Duration, double FuelKg)
{
    public double DistanceKm { get; } = DistanceKm >= 1 ? DistanceKm : throw new GlobalExceptions(HttpStatusCode.BadRequest, "Distance must be at least 1 km.");
    public TimeSpan Duration { get; } = Duration >= TimeSpan.Zero ? Duration : throw new GlobalExceptions(HttpStatusCode.BadRequest, "Duration must be positive.");
    public double FuelKg { get; } = FuelKg >= 0 ? FuelKg : throw new GlobalExceptions(HttpStatusCode.BadRequest, "Fuel must be non-negative.");
}