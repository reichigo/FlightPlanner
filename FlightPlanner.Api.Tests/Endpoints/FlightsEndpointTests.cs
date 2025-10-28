using System.Net;
using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using Shouldly;
using Xunit;

namespace FlightPlanner.Api.Tests.Endpoints;

public class FlightsEndpointTests(IntegrationTestWebAppFactory factory) : IntegrationTestBase(factory)
{
    // Generate unique 3-letter IATA code using first 2 chars + random letter
    private static string GenerateUniqueIata(string prefix)
    {
        var random = new Random();
        var letter = (char)('A' + random.Next(0, 26));
        return $"{prefix.Substring(0, Math.Min(2, prefix.Length))}{letter}";
    }

    private static string GenerateUniqueModel(string prefix) => $"{prefix}-{Guid.NewGuid().ToString().Substring(0, 8)}";

    private async Task<AirportResponse> CreateTestAirport(string iata, string name, double lat, double lon)
    {
        var request = new CreateAirportRequest(iata, name, lat, lon);
        var response = await PostAsync("/airports", request);
        return (await response.Content.ReadFromJsonAsync<AirportResponse>(JsonOptions))!;
    }

    private async Task<AircraftResponse> CreateTestAircraft(string model, double speed, double fuelBurn, double takeoffFuel)
    {
        var request = new CreateAircraftRequest(model, speed, fuelBurn, takeoffFuel);
        var response = await PostAsync("/aircrafts", request);
        return (await response.Content.ReadFromJsonAsync<AircraftResponse>(JsonOptions))!;
    }

    [Fact]
    public async Task CreateFlight_WithValidData_ShouldReturnCreatedWithEstimates()
    {
        // Arrange
        var origin = await CreateTestAirport(GenerateUniqueIata("NYC"), "New York Airport", 40.7128, -74.0060);
        var destination = await CreateTestAirport(GenerateUniqueIata("LAC"), "Los Angeles Airport", 34.0522, -118.2437);
        var aircraft = await CreateTestAircraft(GenerateUniqueModel("Test Boeing 737"), 450, 2500, 500);

        var request = new CreateFlightRequest(
            origin.Id,
            destination.Id,
            aircraft.Id,
            DateTime.UtcNow.AddDays(1)
        );

        // Act
        var response = await PostAsync("/flights", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        
        var flight = await response.Content.ReadFromJsonAsync<FlightResponse>(JsonOptions);
        flight.ShouldNotBeNull();
        flight.Estimates.ShouldNotBeNull();
        flight.Estimates.DistanceKm.ShouldBeGreaterThan(0);
        flight.Estimates.Duration.ShouldBeGreaterThan(TimeSpan.Zero);
        flight.Estimates.FuelKg.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task CreateFlight_WithSameOriginAndDestination_ShouldReturnBadRequest()
    {
        // Arrange
        var airport = await CreateTestAirport(GenerateUniqueIata("DEN"), "Denver International Airport", 39.8561, -104.6737);
        var aircraft = await CreateTestAircraft(GenerateUniqueModel("Test Airbus A320"), 447, 2400, 450);

        var request = new CreateFlightRequest(
            airport.Id,
            airport.Id, // Same as origin
            aircraft.Id,
            DateTime.UtcNow.AddDays(1)
        );

        // Act
        var response = await PostAsync("/flights", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateFlight_WithNonExistentOrigin_ShouldReturnNotFound()
    {
        // Arrange
        var destination = await CreateTestAirport(GenerateUniqueIata("PHX"), "Phoenix Sky Harbor", 33.4352, -112.0101);
        var aircraft = await CreateTestAircraft(GenerateUniqueModel("Test Boeing 777"), 490, 6800, 1200);

        var request = new CreateFlightRequest(
            Guid.NewGuid(), // Non-existent origin
            destination.Id,
            aircraft.Id,
            DateTime.UtcNow.AddDays(1)
        );

        // Act
        var response = await PostAsync("/flights", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateFlight_WithNonExistentDestination_ShouldReturnNotFound()
    {
        // Arrange
        var origin = await CreateTestAirport(GenerateUniqueIata("LAS"), "Las Vegas McCarran", 36.0840, -115.1537);
        var aircraft = await CreateTestAircraft(GenerateUniqueModel("Test Airbus A380"), 488, 12000, 2000);

        var request = new CreateFlightRequest(
            origin.Id,
            Guid.NewGuid(), // Non-existent destination
            aircraft.Id,
            DateTime.UtcNow.AddDays(1)
        );

        // Act
        var response = await PostAsync("/flights", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateFlight_WithNonExistentAircraft_ShouldReturnNotFound()
    {
        // Arrange
        var origin = await CreateTestAirport(GenerateUniqueIata("IAH"), "Houston George Bush", 29.9902, -95.3368);
        var destination = await CreateTestAirport(GenerateUniqueIata("MCO"), "Orlando International", 28.4312, -81.3081);

        var request = new CreateFlightRequest(
            origin.Id,
            destination.Id,
            Guid.NewGuid(), // Non-existent aircraft
            DateTime.UtcNow.AddDays(1)
        );

        // Act
        var response = await PostAsync("/flights", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllFlights_ShouldReturnListWithEstimates()
    {
        // Arrange
        var origin = await CreateTestAirport(GenerateUniqueIata("PDX"), "Portland International", 45.5898, -122.5951);
        var destination = await CreateTestAirport(GenerateUniqueIata("SLC"), "Salt Lake City International", 40.7899, -111.9791);
        var aircraft = await CreateTestAircraft(GenerateUniqueModel("Test Boeing 787"), 488, 5400, 900);

        await PostAsync("/flights", new CreateFlightRequest(origin.Id, destination.Id, aircraft.Id, DateTime.UtcNow.AddDays(1)));

        // Act
        var flights = await GetAsync<List<FlightResponse>>("/flights");

        // Assert
        flights.ShouldNotBeNull();
        flights.Count.ShouldBeGreaterThanOrEqualTo(1);
        flights.All(f => f.Estimates != null).ShouldBeTrue();
    }

    [Fact]
    public async Task GetFlightById_WithValidId_ShouldReturnFlight()
    {
        // Arrange
        var origin = await CreateTestAirport(GenerateUniqueIata("MSP"), "Minneapolis-St Paul", 44.8848, -93.2223);
        var destination = await CreateTestAirport(GenerateUniqueIata("DTW"), "Detroit Metropolitan", 42.2162, -83.3554);
        var aircraft = await CreateTestAircraft(GenerateUniqueModel("Test Embraer E190"), 447, 1800, 300);

        var createResponse = await PostAsync("/flights", new CreateFlightRequest(origin.Id, destination.Id, aircraft.Id, DateTime.UtcNow.AddDays(1)));
        var created = await createResponse.Content.ReadFromJsonAsync<FlightResponse>(JsonOptions);

        // Act
        var flight = await GetAsync<FlightResponse>($"/flights/{created!.Id}");

        // Assert
        flight.ShouldNotBeNull();
        flight.Id.ShouldBe(created.Id);
        flight.Estimates.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetFlightById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/flights/{invalidId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateFlight_WithValidData_ShouldReturnUpdated()
    {
        // Arrange
        var origin = await CreateTestAirport(GenerateUniqueIata("CLT"), "Charlotte Douglas", 35.2144, -80.9473);
        var destination = await CreateTestAirport(GenerateUniqueIata("PHL"), "Philadelphia International", 39.8744, -75.2424);
        var aircraft = await CreateTestAircraft(GenerateUniqueModel("Test Airbus A321"), 447, 2600, 550);

        var createResponse = await PostAsync("/flights", new CreateFlightRequest(origin.Id, destination.Id, aircraft.Id, DateTime.UtcNow.AddDays(1)));
        var created = await createResponse.Content.ReadFromJsonAsync<FlightResponse>(JsonOptions);

        var updateRequest = new UpdateFlightRequest(
            origin.Id,
            destination.Id,
            aircraft.Id,
            DateTime.UtcNow.AddDays(2)
        );

        // Act
        var response = await PutAsync($"/flights/{created!.Id}", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<FlightResponse>(JsonOptions);
        updated.ShouldNotBeNull();
        updated.DepartureAt.ShouldNotBe(created.DepartureAt);
    }

    [Fact]
    public async Task DeleteFlight_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var origin = await CreateTestAirport(GenerateUniqueIata("BWI"), "Baltimore/Washington", 39.1774, -76.6684);
        var destination = await CreateTestAirport(GenerateUniqueIata("RDU"), "Raleigh-Durham", 35.8801, -78.7880);
        var aircraft = await CreateTestAircraft(GenerateUniqueModel("Test Boeing 737 MAX"), 453, 2300, 480);

        var createResponse = await PostAsync("/flights", new CreateFlightRequest(origin.Id, destination.Id, aircraft.Id, DateTime.UtcNow.AddDays(1)));
        var created = await createResponse.Content.ReadFromJsonAsync<FlightResponse>(JsonOptions);

        // Act
        var response = await DeleteAsync($"/flights/{created!.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify it's deleted
        var getResponse = await Client.GetAsync($"/flights/{created.Id}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateFlight_DistanceCalculation_ShouldBeAccurate()
    {
        // Arrange - JFK to LAX is approximately 3,970 km
        var jfk = await CreateTestAirport(GenerateUniqueIata("JFK"), "John F. Kennedy", 40.6413, -73.7781);
        var lax = await CreateTestAirport(GenerateUniqueIata("LAX"), "Los Angeles International", 33.9416, -118.4085);
        var aircraft = await CreateTestAircraft(GenerateUniqueModel("Test Boeing 737-800"), 450, 2500, 500);

        var request = new CreateFlightRequest(jfk.Id, lax.Id, aircraft.Id, DateTime.UtcNow.AddDays(1));

        // Act
        var response = await PostAsync("/flights", request);
        var flight = await response.Content.ReadFromJsonAsync<FlightResponse>(JsonOptions);

        // Assert
        flight.ShouldNotBeNull();
        flight.Estimates.ShouldNotBeNull();
        // Distance should be approximately 3,970 km (allow 10% margin)
        flight.Estimates.DistanceKm.ShouldBeInRange(3500, 4400);
    }
}

