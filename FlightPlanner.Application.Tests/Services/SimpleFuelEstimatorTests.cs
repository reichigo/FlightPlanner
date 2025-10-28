using FlightPlanner.Application.Services;
using FlightPlanner.Application.Tests.Fixtures;
using Shouldly;
using Xunit;

namespace FlightPlanner.Application.Tests.Services;

public class SimpleFuelEstimatorTests : TestFixtureBase
{
    private readonly SimpleFuelEstimator _sut;

    public SimpleFuelEstimatorTests()
    {
        _sut = new SimpleFuelEstimator();
    }

    [Fact]
    public void Estimate_WithValidInputs_ShouldReturnCorrectFuelAmount()
    {
        // Arrange
        var aircraft = CreateTestAircraft("Boeing 737-800", cruiseSpeed: 450, fuelBurn: 2500, takeoffFuel: 20000);
        var distanceKm = 1000.0;

        // Act
        var estimates = _sut.Estimate(aircraft, distanceKm);

        // Assert
        estimates.FuelKg.ShouldBeGreaterThan(0);
        estimates.DistanceKm.ShouldBe(distanceKm);
        estimates.Duration.ShouldBeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public void Estimate_WithZeroDistance_ShouldReturnMinimalFuel()
    {
        // Arrange
        var aircraft = CreateTestAircraft("Boeing 737-800", cruiseSpeed: 450, fuelBurn: 2500, takeoffFuel: 20000);
        var distanceKm = 1.0; // Minimum distance is 1 km

        // Act
        var estimates = _sut.Estimate(aircraft, distanceKm);

        // Assert
        estimates.FuelKg.ShouldBeGreaterThanOrEqualTo(0);
        estimates.DistanceKm.ShouldBe(distanceKm);
    }

    [Fact]
    public void Estimate_WithLongDistance_ShouldScaleProportionally()
    {
        // Arrange
        var aircraft = CreateTestAircraft("Boeing 737-800", cruiseSpeed: 450, fuelBurn: 2500, takeoffFuel: 20000);
        var shortDistance = 500.0;
        var longDistance = 1000.0;

        // Act
        var shortEstimates = _sut.Estimate(aircraft, shortDistance);
        var longEstimates = _sut.Estimate(aircraft, longDistance);

        // Assert
        longEstimates.FuelKg.ShouldBeGreaterThan(shortEstimates.FuelKg);
        longEstimates.Duration.ShouldBeGreaterThan(shortEstimates.Duration);
        // Longer distance should require more fuel (but not exactly double due to fixed takeoff fuel)
        longEstimates.FuelKg.ShouldBeGreaterThan(shortEstimates.FuelKg * 1.05);
    }

    [Fact]
    public void Estimate_WithDifferentAircraft_ShouldReturnDifferentResults()
    {
        // Arrange
        var efficientAircraft = CreateTestAircraft("Efficient Plane", cruiseSpeed: 450, fuelBurn: 1500, takeoffFuel: 15000);
        var inefficientAircraft = CreateTestAircraft("Inefficient Plane", cruiseSpeed: 450, fuelBurn: 3500, takeoffFuel: 25000);
        var distanceKm = 1000.0;

        // Act
        var efficientEstimates = _sut.Estimate(efficientAircraft, distanceKm);
        var inefficientEstimates = _sut.Estimate(inefficientAircraft, distanceKm);

        // Assert
        inefficientEstimates.FuelKg.ShouldBeGreaterThan(efficientEstimates.FuelKg);
    }
}

