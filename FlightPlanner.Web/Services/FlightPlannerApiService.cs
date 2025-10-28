using System.Text;
using System.Text.Json;
using FlightPlanner.Web.Exceptions;
using FlightPlanner.Web.Models;

namespace FlightPlanner.Web.Services;

public class FlightPlannerApiService(IHttpClientFactory httpClientFactory) : IFlightPlannerApiService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private async Task<string> GetErrorMessageAsync(HttpResponseMessage response)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _jsonOptions);
            return errorResponse?.Message ?? response.ReasonPhrase ?? "Unknown error";
        }
        catch
        {
            return response.ReasonPhrase ?? "Unknown error";
        }
    }

    private async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var message = await GetErrorMessageAsync(response);
            throw new ApiException((int)response.StatusCode, message);
        }
    }

    // Airports
    public async Task<List<AirportViewModel>> GetAirportsAsync()
    {
        using var client = httpClientFactory.CreateClient("FlightPlannerApi");
        var response = await client.GetAsync("/airports/search?pageSize=100");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PagedResponse<AirportViewModel>>(content, _jsonOptions);
        return result?.Items ?? new List<AirportViewModel>();
    }

    public async Task<AirportViewModel?> GetAirportAsync(Guid id)
    {
        using var client = httpClientFactory.CreateClient("FlightPlannerApi");
        var response = await client.GetAsync($"/airports/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AirportViewModel>(content, _jsonOptions);
    }

    public async Task<AirportViewModel> CreateAirportAsync(CreateAirportViewModel model)
    {
        using var client = httpClientFactory.CreateClient("FlightPlannerApi");
        var json = JsonSerializer.Serialize(model, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/airports", content);
        await EnsureSuccessOrThrowAsync(response);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AirportViewModel>(responseContent, _jsonOptions)!;
    }

    public async Task<AirportViewModel> UpdateAirportAsync(Guid id, CreateAirportViewModel model)
    {
        using var client = httpClientFactory.CreateClient("FlightPlannerApi");
        var json = JsonSerializer.Serialize(model, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync($"/airports/{id}", content);
        await EnsureSuccessOrThrowAsync(response);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AirportViewModel>(responseContent, _jsonOptions)!;
    }

    // Aircrafts
    public async Task<List<AircraftViewModel>> GetAircraftsAsync()
    {
        using var client = httpClientFactory.CreateClient("FlightPlannerApi");
        var response = await client.GetAsync("/aircrafts");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<AircraftViewModel>>(content, _jsonOptions) ?? new List<AircraftViewModel>();
    }

    public async Task<AircraftViewModel?> GetAircraftAsync(Guid id)
    {
        using var client = httpClientFactory.CreateClient("FlightPlannerApi");
        var response = await client.GetAsync($"/aircrafts/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AircraftViewModel>(content, _jsonOptions);
    }

    public async Task<AircraftViewModel> CreateAircraftAsync(CreateAircraftViewModel model)
    {
        using var client = httpClientFactory.CreateClient("FlightPlannerApi");
        var json = JsonSerializer.Serialize(model, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/aircrafts", content);
        await EnsureSuccessOrThrowAsync(response);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<AircraftViewModel>(responseContent, _jsonOptions)!;
    }

    // Flights
    public async Task<List<FlightViewModel>> GetFlightsAsync()
    {
        using var client = httpClientFactory.CreateClient("FlightPlannerApi");
        var response = await client.GetAsync("/flights");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<FlightViewModel>>(content, _jsonOptions) ?? new List<FlightViewModel>();
    }

    public async Task<FlightViewModel?> GetFlightAsync(Guid id)
    {
        using var client = httpClientFactory.CreateClient("FlightPlannerApi");
        var response = await client.GetAsync($"/flights/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<FlightViewModel>(content, _jsonOptions);
    }

    public async Task<FlightViewModel> CreateFlightAsync(CreateFlightViewModel model)
    {
        using var client = httpClientFactory.CreateClient("FlightPlannerApi");
        var json = JsonSerializer.Serialize(model, _jsonOptions);

        // DEBUG: Log the JSON being sent
        Console.WriteLine($"DEBUG - JSON being sent to API: {json}");

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/flights", content);
        await EnsureSuccessOrThrowAsync(response);
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<FlightViewModel>(responseContent, _jsonOptions)!;
    }

    public async Task DeleteFlightAsync(Guid id)
    {
        using var client = httpClientFactory.CreateClient("FlightPlannerApi");
        var response = await client.DeleteAsync($"/flights/{id}");
        response.EnsureSuccessStatusCode();
    }

    // Reports
    public async Task<FlightReportViewModel> GetFlightReportAsync()
    {
        using var client = httpClientFactory.CreateClient("FlightPlannerApi");
        var response = await client.GetAsync("/reports");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<FlightReportViewModel>(content, _jsonOptions) ?? new FlightReportViewModel();
    }

    private sealed record PagedResponse<T>(
        List<T> Items,
        int Page,
        int PageSize,
        int TotalCount,
        int TotalPages);

    private sealed record ErrorResponse(string Message);
}

