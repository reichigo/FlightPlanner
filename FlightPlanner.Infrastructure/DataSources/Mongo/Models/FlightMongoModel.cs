using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightPlanner.Infrastructure.DataSources.Mongo.Models;

public class FlightMongoModel
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }

    [BsonElement("originId")]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid OriginId { get; set; }

    [BsonElement("destinationId")]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid DestinationId { get; set; }

    [BsonElement("aircraftId")]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid AircraftId { get; set; }

    [BsonElement("departureAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? DepartureAt { get; set; }
}

