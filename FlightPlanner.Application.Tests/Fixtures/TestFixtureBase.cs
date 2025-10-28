using FlightPlanner.Domain.Entities;

namespace FlightPlanner.Application.Tests.Fixtures;

public class TestFixtureBase
{
    protected static Airport CreateTestAirport(string iataCode = "JFK", string name = "John F. Kennedy International Airport", double latitude = 40.6413, double longitude = -73.7781)
    {
        var iata = new IataCode(iataCode);
        var location = new GeoCoordinate(latitude, longitude);
        var airport = new Airport(iata, name, location);
        
        // Set ID using reflection
        var idProperty = typeof(Airport).GetProperty("Id");
        idProperty?.SetValue(airport, Guid.NewGuid());
        
        return airport;
    }

    protected static Aircraft CreateTestAircraft(string model = "Boeing 737-800", double cruiseSpeed = 450, double fuelBurn = 2500, double takeoffFuel = 20000)
    {
        var aircraft = new Aircraft(model, cruiseSpeed, fuelBurn, takeoffFuel);
        
        // Set ID using reflection
        var idProperty = typeof(Aircraft).GetProperty("Id");
        idProperty?.SetValue(aircraft, Guid.NewGuid());
        
        return aircraft;
    }

    protected static Flight CreateTestFlight(Guid originId, Guid destinationId, Guid aircraftId, DateTime? departureAt = null)
    {
        var flight = new Flight(originId, destinationId, aircraftId, departureAt ?? DateTime.UtcNow.AddDays(1));

        // Set ID using reflection - try multiple possible field names
        var type = typeof(Flight);
        var backingField = type.GetField("<Id>k__BackingField",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        if (backingField == null)
        {
            // Try alternative backing field name
            backingField = type.GetField("_id",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        }

        if (backingField != null)
        {
            backingField.SetValue(flight, Guid.NewGuid());
        }
        else
        {
            // If we can't set the ID via reflection, the test will use Guid.Empty
            // This is acceptable for most tests
        }

        return flight;
    }
}

