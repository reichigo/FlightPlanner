using System.Net;
using FlightPlanner.Domain.Exceptions;

namespace FlightPlanner.Domain.Entities;

public sealed record GeoCoordinate(double Latitude, double Longitude)
{
    public double Latitude { get; } =
        (Latitude >= -90 && Latitude <= 90)
            ? Latitude
            : throw new GlobalExceptions(HttpStatusCode.BadRequest, "Latitude must be between -90 and 90.");

    public double Longitude { get; } =
        (Longitude >= -180 && Longitude <= 180)
            ? Longitude
            : throw new GlobalExceptions(HttpStatusCode.BadRequest, "Longitude must be between -180 and 180.");
}