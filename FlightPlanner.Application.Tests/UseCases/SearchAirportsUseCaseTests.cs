using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Tests.Fixtures;
using FlightPlanner.Application.UseCases;
using FlightPlanner.Domain.Repositories;
using Moq;
using Shouldly;
using Xunit;

namespace FlightPlanner.Application.Tests.UseCases;

public class SearchAirportsUseCaseTests : TestFixtureBase
{
    private readonly Mock<IAirportRepository> _airportRepositoryMock;
    private readonly SearchAirportsUseCase _sut;

    public SearchAirportsUseCaseTests()
    {
        _airportRepositoryMock = new Mock<IAirportRepository>();
        _sut = new SearchAirportsUseCase(_airportRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithMatchingAirports_ShouldReturnPagedResponse()
    {
        // Arrange
        var query = new SearchAirportsQuery("John", 1, 10);
        var airport1 = CreateTestAirport("JFK", "John F. Kennedy International Airport");
        var airport2 = CreateTestAirport("SJC", "San Jose International Airport");
        var airports = new List<Domain.Entities.Airport> { airport1, airport2 };

        _airportRepositoryMock
            .Setup(x => x.SearchAsync("John", 0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(airports);

        _airportRepositoryMock
            .Setup(x => x.CountAsync("John", It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        // Act
        var result = await _sut.ExecuteAsync(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(2);
        result.Items[0].Name.ShouldBe(airport1.Name);
        result.Items[1].Name.ShouldBe(airport2.Name);
        result.TotalCount.ShouldBe(2);
        result.Page.ShouldBe(1);
        result.PageSize.ShouldBe(10);

        _airportRepositoryMock.Verify(x => x.SearchAsync("John", 0, 10, It.IsAny<CancellationToken>()), Times.Once);
        _airportRepositoryMock.Verify(x => x.CountAsync("John", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNoMatches_ShouldReturnEmptyPagedResponse()
    {
        // Arrange
        var query = new SearchAirportsQuery("NonExistent", 1, 10);
        var airports = new List<Domain.Entities.Airport>();

        _airportRepositoryMock
            .Setup(x => x.SearchAsync("NonExistent", 0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(airports);

        _airportRepositoryMock
            .Setup(x => x.CountAsync("NonExistent", It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _sut.ExecuteAsync(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(0);
        result.TotalCount.ShouldBe(0);

        _airportRepositoryMock.Verify(x => x.SearchAsync("NonExistent", 0, 10, It.IsAny<CancellationToken>()), Times.Once);
        _airportRepositoryMock.Verify(x => x.CountAsync("NonExistent", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptySearchTerm_ShouldCallRepository()
    {
        // Arrange
        var query = new SearchAirportsQuery("", 1, 10);
        var airports = new List<Domain.Entities.Airport>();

        _airportRepositoryMock
            .Setup(x => x.SearchAsync("", 0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(airports);

        _airportRepositoryMock
            .Setup(x => x.CountAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _sut.ExecuteAsync(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        _airportRepositoryMock.Verify(x => x.SearchAsync("", 0, 10, It.IsAny<CancellationToken>()), Times.Once);
    }
}

