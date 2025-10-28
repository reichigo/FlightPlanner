using FlightPlanner.Application.Tests.Fixtures;
using FlightPlanner.Application.UseCases;
using FlightPlanner.Domain.Repositories;
using Moq;
using Shouldly;
using Xunit;

namespace FlightPlanner.Application.Tests.UseCases;

public class GetAllAircraftsUseCaseTests : TestFixtureBase
{
    private readonly Mock<IAircraftRepository> _aircraftRepositoryMock;
    private readonly GetAllAircraftsUseCase _sut;

    public GetAllAircraftsUseCaseTests()
    {
        _aircraftRepositoryMock = new Mock<IAircraftRepository>();
        _sut = new GetAllAircraftsUseCase(_aircraftRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingAircrafts_ShouldReturnList()
    {
        // Arrange
        var aircraft1 = CreateTestAircraft("Boeing 737-800");
        var aircraft2 = CreateTestAircraft("Airbus A320");
        var aircrafts = new List<Domain.Entities.Aircraft> { aircraft1, aircraft2 };

        _aircraftRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(aircrafts);

        // Act
        var result = await _sut.ExecuteAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].Model.ShouldBe(aircraft1.Model);
        result[1].Model.ShouldBe(aircraft2.Model);

        _aircraftRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoAircrafts_ShouldReturnEmptyList()
    {
        // Arrange
        var aircrafts = new List<Domain.Entities.Aircraft>();

        _aircraftRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(aircrafts);

        // Act
        var result = await _sut.ExecuteAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);

        _aircraftRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

