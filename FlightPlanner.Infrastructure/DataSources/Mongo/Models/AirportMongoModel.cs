using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightPlanner.Infrastructure.DataSources.Mongo.Models;

public class AirportMongoModel
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }

    [BsonElement("iata")]
    public string Iata { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("latitude")]
    public double Latitude { get; set; }

    [BsonElement("longitude")]
    public double Longitude { get; set; }
}

