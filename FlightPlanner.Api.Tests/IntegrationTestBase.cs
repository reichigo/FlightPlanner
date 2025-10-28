using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace FlightPlanner.Api.Tests;

public abstract class IntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly HttpClient Client;
    protected readonly IntegrationTestWebAppFactory Factory;
    protected readonly JsonSerializerOptions JsonOptions;

    protected IntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    protected async Task<TResponse?> GetAsync<TResponse>(string url)
    {
        var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
    }

    protected async Task<HttpResponseMessage> PostAsync<TRequest>(string url, TRequest request)
    {
        return await Client.PostAsJsonAsync(url, request, JsonOptions);
    }

    protected async Task<TResponse?> PostAndGetAsync<TRequest, TResponse>(string url, TRequest request)
    {
        var response = await PostAsync(url, request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
    }

    protected async Task<HttpResponseMessage> PutAsync<TRequest>(string url, TRequest request)
    {
        return await Client.PutAsJsonAsync(url, request, JsonOptions);
    }

    protected async Task<TResponse?> PutAndGetAsync<TRequest, TResponse>(string url, TRequest request)
    {
        var response = await PutAsync(url, request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(string url)
    {
        return await Client.DeleteAsync(url);
    }
}

