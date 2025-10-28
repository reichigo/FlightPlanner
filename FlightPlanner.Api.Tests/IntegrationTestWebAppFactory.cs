using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MongoDb;
using Xunit;

namespace FlightPlanner.Api.Tests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
        .WithImage("mongo:latest")
        .WithPortBinding(27017, true)
        .Build();

    public string MongoConnectionString => _mongoContainer.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Override MongoDB connection string for tests
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["MongoDB:ConnectionString"] = MongoConnectionString,
                ["MongoDB:DatabaseName"] = "FlightPlannerTestDb"
            });
        });

        builder.ConfigureServices(services =>
        {
            
        });
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
    }
}

