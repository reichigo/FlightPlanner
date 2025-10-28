using System.Net;
using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using Shouldly;
using Xunit;

namespace FlightPlanner.Api.Tests.Endpoints;

public class AircraftsEndpointTests(IntegrationTestWebAppFactory factory) : IntegrationTestBase(factory)
{
    private static string GenerateUniqueModel(string prefix) => $"{prefix}-{Guid.NewGuid().ToString().Substring(0, 8)}";

    [Fact]
    public async Task CreateAircraft_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var uniqueModel = GenerateUniqueModel("Boeing 737-800");
        var request = new CreateAircraftRequest(
            uniqueModel,
            450,      // CruiseSpeedKts
            2500,     // FuelBurnPerHourKg
            500       // TakeoffFuelKg
        );

        // Act
        var response = await PostAsync("/aircrafts", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        
        var aircraft = await response.Content.ReadFromJsonAsync<AircraftResponse>(JsonOptions);
        aircraft.ShouldNotBeNull();
        aircraft.Model.ShouldBe(uniqueModel);
        aircraft.CruiseSpeedKts.ShouldBe(450);
        aircraft.FuelBurnPerHourKg.ShouldBe(2500);
        aircraft.TakeoffFuelKg.ShouldBe(500);
        aircraft.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateAircraft_WithDuplicateModel_ShouldReturnConflict()
    {
        // Arrange - Use unique model with timestamp to avoid conflicts
        var uniqueModel = $"Airbus A320-{DateTime.UtcNow.Ticks % 10000}";
        var request = new CreateAircraftRequest(uniqueModel, 447, 2400, 450);

        // Act - Create first time
        await PostAsync("/aircrafts", request);

        // Act - Try to create again with same model
        var response = await PostAsync("/aircrafts", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetAllAircrafts_ShouldReturnList()
    {
        // Arrange - Create some aircrafts
        await PostAsync("/aircrafts", new CreateAircraftRequest(GenerateUniqueModel("Boeing 777-300ER"), 490, 6800, 1200));
        await PostAsync("/aircrafts", new CreateAircraftRequest(GenerateUniqueModel("Airbus A380"), 488, 12000, 2000));

        // Act
        var aircrafts = await GetAsync<List<AircraftResponse>>("/aircrafts");

        // Assert
        aircrafts.ShouldNotBeNull();
        aircrafts.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetAircraftById_WithValidId_ShouldReturnAircraft()
    {
        // Arrange
        var uniqueModel = GenerateUniqueModel("Boeing 787-9");
        var createRequest = new CreateAircraftRequest(uniqueModel, 488, 5400, 900);
        var createResponse = await PostAsync("/aircrafts", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<AircraftResponse>(JsonOptions);

        // Act
        var aircraft = await GetAsync<AircraftResponse>($"/aircrafts/{created!.Id}");

        // Assert
        aircraft.ShouldNotBeNull();
        aircraft.Id.ShouldBe(created.Id);
        aircraft.Model.ShouldBe(uniqueModel);
    }

    [Fact]
    public async Task GetAircraftById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/aircrafts/{invalidId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    
    [Fact]
    public async Task CreateAircraft_WithNegativeSpeed_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateAircraftRequest("Test Aircraft", -100, 2000, 500);

        // Act
        var response = await PostAsync("/aircrafts", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAircraft_WithNegativeFuelBurn_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateAircraftRequest("Test Aircraft", 450, -2000, 500);

        // Act
        var response = await PostAsync("/aircrafts", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAircraft_WithEmptyModel_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateAircraftRequest("", 450, 2000, 500);

        // Act
        var response = await PostAsync("/aircrafts", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}

