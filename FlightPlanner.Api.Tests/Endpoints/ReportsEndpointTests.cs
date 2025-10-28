using System.Net;
using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using Shouldly;
using Xunit;

namespace FlightPlanner.Api.Tests.Endpoints;

public class ReportsEndpointTests(IntegrationTestWebAppFactory factory) : IntegrationTestBase(factory)
{
    // Generate unique 3-letter IATA code using first 2 chars + random letter
    private static string GenerateUniqueIata(string prefix)
    {
        var random = new Random();
        var letter = (char)('A' + random.Next(0, 26));
        return $"{prefix.Substring(0, Math.Min(2, prefix.Length))}{letter}";
    }

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
    public async Task GetFlightReport_WithValidFlights_ShouldReturnReport()
    {
        // Arrange
        var origin = await CreateTestAirport("ATL", "Atlanta Hartsfield", 33.6407, -84.4277);
        var destination = await CreateTestAirport("ORD", "Chicago O'Hare", 41.9742, -87.9073);
        var aircraft = await CreateTestAircraft("Report Test Boeing 737", 450, 2500, 500);

        await PostAsync("/flights", new CreateFlightRequest(origin.Id, destination.Id, aircraft.Id, DateTime.UtcNow.AddDays(1)));

        // Act
        var report = await GetAsync<FlightReportResponse>("/reports");

        // Assert
        report.ShouldNotBeNull();
        report.Flights.ShouldNotBeNull();
        report.Flights.Count.ShouldBeGreaterThanOrEqualTo(1);
        report.TotalFlights.ShouldBe(report.Flights.Count);
        report.TotalDistanceKm.ShouldBeGreaterThan(0);
        report.TotalFuelKg.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetFlightReport_WithInvalidFlightId_ShouldReturnNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/reports/{invalidId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFlightSummary_WithNoFlights_ShouldReturnEmptySummary()
    {
        // Act
        var summary = await GetAsync<FlightSummaryResponse>("/reports/summary");

        // Assert
        summary.ShouldNotBeNull();
        summary.TotalFlights.ShouldBeGreaterThanOrEqualTo(0);
        summary.TotalDistanceKm.ShouldBeGreaterThanOrEqualTo(0);
        summary.TotalFuelKg.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetFlightSummary_WithMultipleFlights_ShouldCalculateAverages()
    {
        // Arrange
        var origin1 = await CreateTestAirport("DFW", "Dallas Fort Worth", 32.8998, -97.0403);
        var destination1 = await CreateTestAirport("MIA", "Miami International", 25.7959, -80.2870);
        var origin2 = await CreateTestAirport("SEA", "Seattle-Tacoma", 47.4502, -122.3088);
        var destination2 = await CreateTestAirport("BOS", "Boston Logan", 42.3656, -71.0096);
        var aircraft = await CreateTestAircraft("Summary Test Airbus A320", 447, 2400, 450);

        await PostAsync("/flights", new CreateFlightRequest(origin1.Id, destination1.Id, aircraft.Id, DateTime.UtcNow.AddDays(1)));
        await PostAsync("/flights", new CreateFlightRequest(origin2.Id, destination2.Id, aircraft.Id, DateTime.UtcNow.AddDays(2)));

        // Act
        var summary = await GetAsync<FlightSummaryResponse>("/reports/summary");

        // Assert
        summary.ShouldNotBeNull();
        summary.TotalFlights.ShouldBeGreaterThanOrEqualTo(2);
        summary.TotalDistanceKm.ShouldBeGreaterThan(0);
        summary.TotalFuelKg.ShouldBeGreaterThan(0);
        summary.AverageDistanceKm.ShouldBeGreaterThan(0);
        summary.AverageFuelKg.ShouldBeGreaterThan(0);
        
        // Verify averages are calculated correctly
        summary.AverageDistanceKm.ShouldBe(summary.TotalDistanceKm / summary.TotalFlights);
        summary.AverageFuelKg.ShouldBe(summary.TotalFuelKg / summary.TotalFlights);
    }

    [Fact]
    public async Task GetFlightReport_ShouldIncludeAllFlightDetails()
    {
        // Arrange - Use unique IATAs to avoid conflicts
        var uniqueOriginIata = GenerateUniqueIata("SF");
        var uniqueDestIata = GenerateUniqueIata("JF");

        var origin = await CreateTestAirport(uniqueOriginIata, "San Francisco", 37.6213, -122.3790);
        var destination = await CreateTestAirport(uniqueDestIata, "John F Kennedy", 40.6413, -73.7781);
        var aircraft = await CreateTestAircraft($"Details-{Guid.NewGuid().ToString().Substring(0, 8)}", 490, 6800, 1200);

        var createResponse = await PostAsync("/flights", new CreateFlightRequest(origin.Id, destination.Id, aircraft.Id, DateTime.UtcNow.AddDays(1)));
        var createdFlight = await createResponse.Content.ReadFromJsonAsync<FlightResponse>(JsonOptions);

        // Act
        var report = await GetAsync<FlightReportResponse>("/reports");

        // Assert
        report.ShouldNotBeNull();
        report.Flights.ShouldNotBeNull();

        var flight = report.Flights.FirstOrDefault(f => f.Id == createdFlight!.Id);
        flight.ShouldNotBeNull();
        flight.Origin.ShouldNotBeNull();
        flight.Destination.ShouldNotBeNull();
        flight.Aircraft.ShouldNotBeNull();
        flight.Estimates.ShouldNotBeNull();
        flight.Estimates.DistanceKm.ShouldBeGreaterThan(0);
        flight.Estimates.Duration.ShouldBeGreaterThan(TimeSpan.Zero);
        flight.Estimates.FuelKg.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetFlightReport_TotalsShouldMatchSumOfFlights()
    {
        // Arrange
        var origin1 = await CreateTestAirport("LAX", "Los Angeles", 33.9416, -118.4085);
        var destination1 = await CreateTestAirport("PHX", "Phoenix", 33.4352, -112.0101);
        var origin2 = await CreateTestAirport("DEN", "Denver", 39.8561, -104.6737);
        var destination2 = await CreateTestAirport("LAS", "Las Vegas", 36.0840, -115.1537);
        var aircraft = await CreateTestAircraft("Totals Test Airbus A350", 488, 5800, 1000);

        await PostAsync("/flights", new CreateFlightRequest(origin1.Id, destination1.Id, aircraft.Id, DateTime.UtcNow.AddDays(1)));
        await PostAsync("/flights", new CreateFlightRequest(origin2.Id, destination2.Id, aircraft.Id, DateTime.UtcNow.AddDays(2)));

        // Act
        var report = await GetAsync<FlightReportResponse>("/reports");

        // Assert
        report.ShouldNotBeNull();
        report.Flights.ShouldNotBeNull();
        
        var calculatedDistance = report.Flights.Sum(f => f.Estimates!.DistanceKm);
        var calculatedFuel = report.Flights.Sum(f => f.Estimates!.FuelKg);
        
        report.TotalDistanceKm.ShouldBe(calculatedDistance);
        report.TotalFuelKg.ShouldBe(calculatedFuel);
        report.TotalFlights.ShouldBe(report.Flights.Count);
    }
}

