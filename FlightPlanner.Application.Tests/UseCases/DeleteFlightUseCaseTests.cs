using FlightPlanner.Application.Tests.Fixtures;
using FlightPlanner.Application.UseCases;
using FlightPlanner.Domain.Repositories;
using Moq;
using Shouldly;
using Xunit;

namespace FlightPlanner.Application.Tests.UseCases;

public class DeleteFlightUseCaseTests : TestFixtureBase
{
    private readonly Mock<IFlightRepository> _flightRepositoryMock;
    private readonly DeleteFlightUseCase _sut;

    public DeleteFlightUseCaseTests()
    {
        _flightRepositoryMock = new Mock<IFlightRepository>();
        _sut = new DeleteFlightUseCase(_flightRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingFlight_ShouldReturnTrue()
    {
        // Arrange
        var flightId = Guid.NewGuid();

        _flightRepositoryMock
            .Setup(x => x.DeleteAsync(flightId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.ExecuteAsync(flightId, CancellationToken.None);

        // Assert
        result.ShouldBeTrue();

        _flightRepositoryMock.Verify(x => x.DeleteAsync(flightId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistingFlight_ShouldReturnFalse()
    {
        // Arrange
        var flightId = Guid.NewGuid();

        _flightRepositoryMock
            .Setup(x => x.DeleteAsync(flightId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _sut.ExecuteAsync(flightId, CancellationToken.None);

        // Assert
        result.ShouldBeFalse();

        _flightRepositoryMock.Verify(x => x.DeleteAsync(flightId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

