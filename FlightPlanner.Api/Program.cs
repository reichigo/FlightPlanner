using FlightPlanner.Api.Endpoint;
using FlightPlanner.Api.Middleware;
using FlightPlanner.Application;
using FlightPlanner.Application.Services;
using FlightPlanner.Application.UseCases;
using FlightPlanner.Domain.Entities.Interfaces;
using FlightPlanner.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();


builder.Services.AddSingleton<IDistanceCalculator, HaversineDistanceCalculator>();
builder.Services.AddSingleton<IFuelEstimator, SimpleFuelEstimator>();

// Use Cases - Flights
builder.Services.AddScoped<ICreateFlightUseCase, CreateFlightUseCase>();
builder.Services.AddScoped<IGetAllFlightsUseCase, GetAllFlightsUseCase>();
builder.Services.AddScoped<IGetFlightUseCase, GetFlightUseCase>();
builder.Services.AddScoped<IUpdateFlightUseCase, UpdateFlightUseCase>();
builder.Services.AddScoped<IDeleteFlightUseCase, DeleteFlightUseCase>();
builder.Services.AddScoped<IGetFlightReportUseCase, GetFlightReportUseCase>();
builder.Services.AddScoped<IGetFlightSummaryUseCase, GetFlightSummaryUseCase>();

// Use Cases - Airports
builder.Services.AddScoped<ICreateAirportUseCase, CreateAirportUseCase>();
builder.Services.AddScoped<IGetAirportUseCase, GetAirportUseCase>();
builder.Services.AddScoped<ISearchAirportsUseCase, SearchAirportsUseCase>();
builder.Services.AddScoped<IUpdateAirportUseCase, UpdateAirportUseCase>();

// Use Cases - Aircrafts
builder.Services.AddScoped<ICreateAircraftUseCase, CreateAircraftUseCase>();
builder.Services.AddScoped<IGetAllAircraftsUseCase, GetAllAircraftsUseCase>();
builder.Services.AddScoped<IGetAircraftUseCase, GetAircraftUseCase>();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map endpoints
AirportsEndpoint.MapAirportsEndpoints(app);
AircraftsEndpoint.MapAircraftsEndpoints(app);
FlightsEndpoint.MapFlightsEndpoints(app);
ReportsEndpoint.MapReportsEndpoints(app);

app.UseMiddleware<GlobalExceptionMiddleware>();

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
