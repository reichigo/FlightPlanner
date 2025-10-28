using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightPlanner.Infrastructure.DataSources.Mongo.Models;

public sealed record AircraftMongoModel
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }

    [BsonElement("model")]
    public string Model { get; set; } = string.Empty;

    [BsonElement("cruiseSpeedKts")]
    public double CruiseSpeedKts { get; set; }

    [BsonElement("fuelBurnPerHourKg")]
    public double FuelBurnPerHourKg { get; set; }

    [BsonElement("takeoffFuelKg")]
    public double TakeoffFuelKg { get; set; }
}

