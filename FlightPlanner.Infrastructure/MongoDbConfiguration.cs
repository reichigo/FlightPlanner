using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace FlightPlanner.Infrastructure;

public static class MongoDbConfiguration
{
    private static bool _isConfigured;

    public static void ConfigureMongoDb()
    {
        if (_isConfigured) return;

        // Configure MongoDB to use standard GUID representation (UUID)
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        
        _isConfigured = true;
    }
}