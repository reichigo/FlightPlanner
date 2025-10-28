using System.Net;
using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using Shouldly;
using Xunit;

namespace FlightPlanner.Api.Tests.Endpoints;

public class AirportsEndpointTests(IntegrationTestWebAppFactory factory) : IntegrationTestBase(factory)
{
    // Generate unique 3-letter IATA code using first 2 chars + random letter
    private static string GenerateUniqueIata(string prefix)
    {
        var random = new Random();
        var letter = (char)('A' + random.Next(0, 26));
        return $"{prefix.Substring(0, Math.Min(2, prefix.Length))}{letter}";
    }

    [Fact]
    public async Task CreateAirport_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var uniqueIata = GenerateUniqueIata("JFK");
        var request = new CreateAirportRequest(
            uniqueIata,
            "John F. Kennedy International Airport",
            40.6413,
            -73.7781
        );

        // Act
        var response = await PostAsync("/airports", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        
        var airport = await response.Content.ReadFromJsonAsync<AirportResponse>(JsonOptions);
        airport.ShouldNotBeNull();
        airport.Iata.ShouldBe(uniqueIata);
        airport.Name.ShouldBe("John F. Kennedy International Airport");
        airport.Latitude.ShouldBe(40.6413);
        airport.Longitude.ShouldBe(-73.7781);
        airport.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateAirport_WithDuplicateIata_ShouldReturnConflict()
    {
        // Arrange - Use valid 3-letter IATA
        var uniqueIata = GenerateUniqueIata("LA");
        var request = new CreateAirportRequest(
            uniqueIata,
            "Los Angeles International Airport",
            33.9416,
            -118.4085
        );

        // Act - Create first time
        await PostAsync("/airports", request);

        // Act - Try to create again with same IATA
        var response = await PostAsync("/airports", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
    

    [Fact]
    public async Task GetAirportById_WithValidId_ShouldReturnAirport()
    {
        // Arrange
        var uniqueIata = GenerateUniqueIata("ATL");
        var createRequest = new CreateAirportRequest(uniqueIata, "Hartsfield-Jackson Atlanta International Airport", 33.6407, -84.4277);
        var createResponse = await PostAsync("/airports", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<AirportResponse>(JsonOptions);

        // Act
        var airport = await GetAsync<AirportResponse>($"/airports/{created!.Id}");

        // Assert
        airport.ShouldNotBeNull();
        airport.Id.ShouldBe(created.Id);
        airport.Iata.ShouldBe(uniqueIata);
    }

    [Fact]
    public async Task GetAirportById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/airports/{invalidId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateAirport_WithValidData_ShouldReturnUpdated()
    {
        // Arrange
        var uniqueIata = GenerateUniqueIata("MIA");
        var createRequest = new CreateAirportRequest(uniqueIata, "Miami International Airport", 25.7959, -80.2870);
        var createResponse = await PostAsync("/airports", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<AirportResponse>(JsonOptions);

        var updateRequest = new UpdateAirportRequest(
            uniqueIata,
            "Miami International Airport - Updated",
            25.7959,
            -80.2870
        );

        // Act
        var response = await PutAsync($"/airports/{created!.Id}", updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var updated = await response.Content.ReadFromJsonAsync<AirportResponse>(JsonOptions);
        updated.ShouldNotBeNull();
        updated.Name.ShouldBe("Miami International Airport - Updated");
    }


    [Fact]
    public async Task CreateAirport_WithInvalidLatitude_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateAirportRequest("TST", "Test Airport", 91.0, -80.0); // Invalid latitude > 90

        // Act
        var response = await PostAsync("/airports", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateAirport_WithInvalidLongitude_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateAirportRequest("TST", "Test Airport", 40.0, -181.0); // Invalid longitude < -180

        // Act
        var response = await PostAsync("/airports", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}

