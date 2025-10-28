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

public class CreateAircraftUseCaseTests : TestFixtureBase
{
    private readonly Mock<IAircraftRepository> _aircraftRepositoryMock;
    private readonly CreateAircraftUseCase _sut;

    public CreateAircraftUseCaseTests()
    {
        _aircraftRepositoryMock = new Mock<IAircraftRepository>();
        _sut = new CreateAircraftUseCase(_aircraftRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateAircraft()
    {
        // Arrange
        var request = new CreateAircraftRequest("Boeing 737-800", 450, 2500, 20000);
        var createdAircraft = CreateTestAircraft();

        _aircraftRepositoryMock
            .Setup(x => x.GetByModelAsync(request.Model, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Aircraft?)null);

        _aircraftRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Aircraft>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdAircraft);

        // Act
        var result = await _sut.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Model.ShouldBe(createdAircraft.Model);
        result.CruiseSpeedKts.ShouldBe(createdAircraft.CruiseSpeedKts);
        result.FuelBurnPerHourKg.ShouldBe(createdAircraft.FuelBurnPerHourKg);
        result.TakeoffFuelKg.ShouldBe(createdAircraft.TakeoffFuelKg);

        _aircraftRepositoryMock.Verify(x => x.GetByModelAsync(request.Model, It.IsAny<CancellationToken>()), Times.Once);
        _aircraftRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Aircraft>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateModel_ShouldThrowGlobalException()
    {
        // Arrange
        var request = new CreateAircraftRequest("Boeing 737-800", 450, 2500, 20000);
        var existingAircraft = CreateTestAircraft();

        _aircraftRepositoryMock
            .Setup(x => x.GetByModelAsync(request.Model, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAircraft);

        // Act & Assert
        var exception = await Should.ThrowAsync<GlobalExceptions>(async () =>
            await _sut.ExecuteAsync(request, CancellationToken.None));

        exception.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        exception.ClientMessage.ShouldContain("already exists");

        _aircraftRepositoryMock.Verify(x => x.GetByModelAsync(request.Model, It.IsAny<CancellationToken>()), Times.Once);
        _aircraftRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Aircraft>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

