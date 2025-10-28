using FlightPlanner.Application.Tests.Fixtures;
using FlightPlanner.Application.UseCases;
using FlightPlanner.Domain.Repositories;
using Moq;
using Shouldly;
using Xunit;

namespace FlightPlanner.Application.Tests.UseCases;

public class GetAirportUseCaseTests : TestFixtureBase
{
    private readonly Mock<IAirportRepository> _airportRepositoryMock;
    private readonly GetAirportUseCase _sut;

    public GetAirportUseCaseTests()
    {
        _airportRepositoryMock = new Mock<IAirportRepository>();
        _sut = new GetAirportUseCase(_airportRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingAirport_ShouldReturnAirportResponse()
    {
        // Arrange
        var airportId = Guid.NewGuid();
        var airport = CreateTestAirport();
        
        // Set the ID
        var idProperty = typeof(Domain.Entities.Airport).GetProperty("Id");
        idProperty?.SetValue(airport, airportId);

        _airportRepositoryMock
            .Setup(x => x.GetByIdAsync(airportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(airport);

        // Act
        var result = await _sut.ExecuteAsync(airportId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(airportId);
        result.Iata.ShouldBe(airport.Iata.Code);
        result.Name.ShouldBe(airport.Name);
        result.Latitude.ShouldBe(airport.Location.Latitude);
        result.Longitude.ShouldBe(airport.Location.Longitude);

        _airportRepositoryMock.Verify(x => x.GetByIdAsync(airportId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingAirport_ShouldReturnNull()
    {
        // Arrange
        var airportId = Guid.NewGuid();

        _airportRepositoryMock
            .Setup(x => x.GetByIdAsync(airportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Airport?)null);

        // Act
        var result = await _sut.ExecuteAsync(airportId, CancellationToken.None);

        // Assert
        result.ShouldBeNull();

        _airportRepositoryMock.Verify(x => x.GetByIdAsync(airportId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

