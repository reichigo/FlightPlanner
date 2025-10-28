namespace FlightPlanner.Infrastructure;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";
    public string DatabaseName { get; set; } = "FlightPlannerDb";
    public string AirportsCollectionName { get; set; } = "Airports";
    public string AircraftsCollectionName { get; set; } = "Aircrafts";
    public string FlightsCollectionName { get; set; } = "Flights";
}