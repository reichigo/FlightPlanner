using FlightPlanner.Application.Services;
using FlightPlanner.Domain.Entities;
using Shouldly;
using Xunit;

namespace FlightPlanner.Application.Tests.Services;

public class HaversineDistanceCalculatorTests
{
    private readonly HaversineDistanceCalculator _sut;

    public HaversineDistanceCalculatorTests()
    {
        _sut = new HaversineDistanceCalculator();
    }

    [Fact]
    public void CalculateDistance_BetweenNewYorkAndLosAngeles_ShouldReturnCorrectDistance()
    {
        // Arrange - JFK to LAX
        var origin = new GeoCoordinate(40.6413, -73.7781);
        var destination = new GeoCoordinate(33.9416, -118.4085);

        // Act
        var distance = _sut.DistanceKm(origin, destination);

        // Assert
        // Expected distance is approximately 3970 km
        distance.ShouldBeInRange(3900, 4000);
    }

    [Fact]
    public void CalculateDistance_BetweenLondonAndParis_ShouldReturnCorrectDistance()
    {
        // Arrange - LHR to CDG
        var origin = new GeoCoordinate(51.4700, -0.4543);
        var destination = new GeoCoordinate(49.0097, 2.5479);

        // Act
        var distance = _sut.DistanceKm(origin, destination);

        // Assert
        // Expected distance is approximately 340 km
        distance.ShouldBeInRange(330, 350);
    }

    [Fact]
    public void CalculateDistance_BetweenSameLocation_ShouldReturnZero()
    {
        // Arrange
        var location = new GeoCoordinate(40.6413, -73.7781);

        // Act
        var distance = _sut.DistanceKm(location, location);

        // Assert
        distance.ShouldBe(0, 0.1);
    }

    [Fact]
    public void CalculateDistance_BetweenEquatorPoints_ShouldReturnCorrectDistance()
    {
        // Arrange - Two points on the equator, 1 degree apart
        var origin = new GeoCoordinate(0, 0);
        var destination = new GeoCoordinate(0, 1);

        // Act
        var distance = _sut.DistanceKm(origin, destination);

        // Assert
        // 1 degree at equator is approximately 111 km
        distance.ShouldBeInRange(110, 112);
    }

    [Fact]
    public void CalculateDistance_BetweenPoles_ShouldReturnCorrectDistance()
    {
        // Arrange - North Pole to South Pole
        var northPole = new GeoCoordinate(90, 0);
        var southPole = new GeoCoordinate(-90, 0);

        // Act
        var distance = _sut.DistanceKm(northPole, southPole);

        // Assert
        // Half the Earth's circumference is approximately 20,000 km
        distance.ShouldBeInRange(19900, 20100);
    }
}

