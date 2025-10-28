using System.Net;
using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Tests.Fixtures;
using FlightPlanner.Application.UseCases;
using FlightPlanner.Domain.Exceptions;
using FlightPlanner.Domain.Repositories;
using Moq;
using Shouldly;
using Xunit;

namespace FlightPlanner.Application.Tests.UseCases;

public class UpdateAirportUseCaseTests : TestFixtureBase
{
    private readonly Mock<IAirportRepository> _airportRepositoryMock;
    private readonly UpdateAirportUseCase _sut;

    public UpdateAirportUseCaseTests()
    {
        _airportRepositoryMock = new Mock<IAirportRepository>();
        _sut = new UpdateAirportUseCase(_airportRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdateAirport()
    {
        // Arrange
        var airportId = Guid.NewGuid();
        var existingAirport = CreateTestAirport("JFK", "Old Name");
        var idProperty = typeof(Domain.Entities.Airport).GetProperty("Id");
        idProperty?.SetValue(existingAirport, airportId);

        var request = new UpdateAirportRequest("JFK", "New Name", 40.6413, -73.7781);

        _airportRepositoryMock
            .Setup(x => x.GetByIdAsync(airportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAirport);

        _airportRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Airport>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAirport);

        // Act
        var result = await _sut.ExecuteAsync(airportId, request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(airportId);

        _airportRepositoryMock.Verify(x => x.GetByIdAsync(airportId, It.IsAny<CancellationToken>()), Times.Once);
        _airportRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Airport>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingAirport_ShouldReturnNull()
    {
        // Arrange
        var airportId = Guid.NewGuid();
        var request = new UpdateAirportRequest("JFK", "New Name", 40.6413, -73.7781);

        _airportRepositoryMock
            .Setup(x => x.GetByIdAsync(airportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Airport?)null);

        // Act
        var result = await _sut.ExecuteAsync(airportId, request, CancellationToken.None);

        // Assert
        result.ShouldBeNull();

        _airportRepositoryMock.Verify(x => x.GetByIdAsync(airportId, It.IsAny<CancellationToken>()), Times.Once);
        _airportRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Airport>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

