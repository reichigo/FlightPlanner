using System.Net;
using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Tests.Fixtures;
using FlightPlanner.Application.UseCases;
using FlightPlanner.Domain.Entities;
using FlightPlanner.Domain.Entities.Interfaces;
using FlightPlanner.Domain.Exceptions;
using FlightPlanner.Domain.Repositories;
using Moq;
using Shouldly;
using Xunit;

namespace FlightPlanner.Application.Tests.UseCases;

public class CreateFlightUseCaseTests : TestFixtureBase
{
    private readonly Mock<IFlightRepository> _flightRepositoryMock;
    private readonly Mock<IAirportRepository> _airportRepositoryMock;
    private readonly Mock<IAircraftRepository> _aircraftRepositoryMock;
    private readonly Mock<IDistanceCalculator> _distanceCalculatorMock;
    private readonly Mock<IFuelEstimator> _fuelEstimatorMock;
    private readonly CreateFlightUseCase _sut;

    public CreateFlightUseCaseTests()
    {
        _flightRepositoryMock = new Mock<IFlightRepository>();
        _airportRepositoryMock = new Mock<IAirportRepository>();
        _aircraftRepositoryMock = new Mock<IAircraftRepository>();
        _distanceCalculatorMock = new Mock<IDistanceCalculator>();
        _fuelEstimatorMock = new Mock<IFuelEstimator>();

        _sut = new CreateFlightUseCase(
            _flightRepositoryMock.Object,
            _airportRepositoryMock.Object,
            _aircraftRepositoryMock.Object,
            _distanceCalculatorMock.Object,
            _fuelEstimatorMock.Object
        );
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateFlight()
    {
        // Arrange
        var origin = CreateTestAirport("JFK");
        var destination = CreateTestAirport("LAX", "Los Angeles International Airport", 33.9416, -118.4085);
        var aircraft = CreateTestAircraft();
        var departureAt = DateTime.UtcNow.AddDays(1);

        var request = new CreateFlightRequest(origin.Id, destination.Id, aircraft.Id, departureAt);
        var createdFlight = CreateTestFlight(origin.Id, destination.Id, aircraft.Id, departureAt);
        var estimates = new FlightEstimates(3970.0, TimeSpan.FromHours(5.3), 15250.0);

        _airportRepositoryMock
            .Setup(x => x.GetByIdAsync(origin.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(origin);

        _airportRepositoryMock
            .Setup(x => x.GetByIdAsync(destination.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(destination);

        _aircraftRepositoryMock
            .Setup(x => x.GetByIdAsync(aircraft.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(aircraft);

        _distanceCalculatorMock
            .Setup(x => x.DistanceKm(It.IsAny<GeoCoordinate>(), It.IsAny<GeoCoordinate>()))
            .Returns(3970.0);

        _fuelEstimatorMock
            .Setup(x => x.Estimate(It.IsAny<Domain.Entities.Aircraft>(), It.IsAny<double>()))
            .Returns(estimates);

        _flightRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Flight>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdFlight);

        // Act
        var result = await _sut.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdFlight.Id);
        result.Origin.Iata.ShouldBe(origin.Iata.Code);
        result.Destination.Iata.ShouldBe(destination.Iata.Code);
        result.Aircraft.Model.ShouldBe(aircraft.Model);
        result.DepartureAt.ShouldBe(departureAt);

        _airportRepositoryMock.Verify(x => x.GetByIdAsync(origin.Id, It.IsAny<CancellationToken>()), Times.Once);
        _airportRepositoryMock.Verify(x => x.GetByIdAsync(destination.Id, It.IsAny<CancellationToken>()), Times.Once);
        _aircraftRepositoryMock.Verify(x => x.GetByIdAsync(aircraft.Id, It.IsAny<CancellationToken>()), Times.Once);
        _flightRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Flight>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidOriginAirport_ShouldThrowGlobalException()
    {
        // Arrange
        var originId = Guid.NewGuid();
        var destinationId = Guid.NewGuid();
        var aircraftId = Guid.NewGuid();
        var departureAt = DateTime.UtcNow.AddDays(1);

        var request = new CreateFlightRequest(originId, destinationId, aircraftId, departureAt);

        _airportRepositoryMock
            .Setup(x => x.GetByIdAsync(originId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Airport?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<GlobalExceptions>(async () =>
            await _sut.ExecuteAsync(request, CancellationToken.None));

        exception.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        exception.ClientMessage.ShouldContain("Origin airport not found");

        _airportRepositoryMock.Verify(x => x.GetByIdAsync(originId, It.IsAny<CancellationToken>()), Times.Once);
        _flightRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Flight>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidDestinationAirport_ShouldThrowGlobalException()
    {
        // Arrange
        var origin = CreateTestAirport("JFK");
        var destinationId = Guid.NewGuid();
        var aircraftId = Guid.NewGuid();
        var departureAt = DateTime.UtcNow.AddDays(1);

        var request = new CreateFlightRequest(origin.Id, destinationId, aircraftId, departureAt);

        _airportRepositoryMock
            .Setup(x => x.GetByIdAsync(origin.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(origin);

        _airportRepositoryMock
            .Setup(x => x.GetByIdAsync(destinationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Airport?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<GlobalExceptions>(async () =>
            await _sut.ExecuteAsync(request, CancellationToken.None));

        exception.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        exception.ClientMessage.ShouldContain("Destination airport not found");

        _airportRepositoryMock.Verify(x => x.GetByIdAsync(origin.Id, It.IsAny<CancellationToken>()), Times.Once);
        _airportRepositoryMock.Verify(x => x.GetByIdAsync(destinationId, It.IsAny<CancellationToken>()), Times.Once);
        _flightRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Flight>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidAircraft_ShouldThrowGlobalException()
    {
        // Arrange
        var origin = CreateTestAirport("JFK");
        var destination = CreateTestAirport("LAX", "Los Angeles International Airport", 33.9416, -118.4085);
        var aircraftId = Guid.NewGuid();
        var departureAt = DateTime.UtcNow.AddDays(1);

        var request = new CreateFlightRequest(origin.Id, destination.Id, aircraftId, departureAt);

        _airportRepositoryMock
            .Setup(x => x.GetByIdAsync(origin.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(origin);

        _airportRepositoryMock
            .Setup(x => x.GetByIdAsync(destination.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(destination);

        _aircraftRepositoryMock
            .Setup(x => x.GetByIdAsync(aircraftId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Aircraft?)null);

        // Act & Assert
        var exception = await Should.ThrowAsync<GlobalExceptions>(async () =>
            await _sut.ExecuteAsync(request, CancellationToken.None));

        exception.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        exception.ClientMessage.ShouldContain("Aircraft not found");

        _airportRepositoryMock.Verify(x => x.GetByIdAsync(origin.Id, It.IsAny<CancellationToken>()), Times.Once);
        _airportRepositoryMock.Verify(x => x.GetByIdAsync(destination.Id, It.IsAny<CancellationToken>()), Times.Once);
        _aircraftRepositoryMock.Verify(x => x.GetByIdAsync(aircraftId, It.IsAny<CancellationToken>()), Times.Once);
        _flightRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Flight>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

