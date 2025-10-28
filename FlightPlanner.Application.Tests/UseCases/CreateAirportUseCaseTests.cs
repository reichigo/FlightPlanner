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

public class CreateAirportUseCaseTests : TestFixtureBase
{
    private readonly Mock<IAirportRepository> _airportRepositoryMock;
    private readonly CreateAirportUseCase _sut;

    public CreateAirportUseCaseTests()
    {
        _airportRepositoryMock = new Mock<IAirportRepository>();
        _sut = new CreateAirportUseCase(_airportRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateAirport()
    {
        // Arrange
        var request = new CreateAirportRequest("JFK", "John F. Kennedy International Airport", 40.6413, -73.7781);
        var createdAirport = CreateTestAirport();

        _airportRepositoryMock
            .Setup(x => x.GetByIataAsync(request.Iata, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Airport?)null);

        _airportRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Airport>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdAirport);

        // Act
        var result = await _sut.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Iata.ShouldBe(createdAirport.Iata.Code);
        result.Name.ShouldBe(createdAirport.Name);
        result.Latitude.ShouldBe(createdAirport.Location.Latitude);
        result.Longitude.ShouldBe(createdAirport.Location.Longitude);

        _airportRepositoryMock.Verify(x => x.GetByIataAsync(request.Iata, It.IsAny<CancellationToken>()), Times.Once);
        _airportRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Airport>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateIata_ShouldThrowGlobalException()
    {
        // Arrange
        var request = new CreateAirportRequest("JFK", "John F. Kennedy International Airport", 40.6413, -73.7781);
        var existingAirport = CreateTestAirport();

        _airportRepositoryMock
            .Setup(x => x.GetByIataAsync(request.Iata, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAirport);

        // Act & Assert
        var exception = await Should.ThrowAsync<GlobalExceptions>(async () =>
            await _sut.ExecuteAsync(request, CancellationToken.None));

        exception.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        exception.ClientMessage.ShouldContain("already exists");

        _airportRepositoryMock.Verify(x => x.GetByIataAsync(request.Iata, It.IsAny<CancellationToken>()), Times.Once);
        _airportRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Airport>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

