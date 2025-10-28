using System.Net;
using FlightPlanner.Domain.Exceptions;

namespace FlightPlanner.Domain.Entities;

public class Airport(IataCode iata, string name, GeoCoordinate location)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public IataCode Iata { get; private set; } = iata;
    public string Name { get; private set; } = !string.IsNullOrWhiteSpace(name) ? name : throw new GlobalExceptions(HttpStatusCode.BadRequest, "Name is required.");
    public GeoCoordinate Location { get; private set; } = location;

    public void Update(IataCode iata, string name, GeoCoordinate location)
    {
        Iata = iata;
        Name = !string.IsNullOrWhiteSpace(name) ? name : throw new GlobalExceptions(HttpStatusCode.BadRequest, "Name is required.");
        Location = location;
    }
}