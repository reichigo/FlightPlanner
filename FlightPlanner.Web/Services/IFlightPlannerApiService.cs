using FlightPlanner.Web.Models;

namespace FlightPlanner.Web.Services;

public interface IFlightPlannerApiService
{
    // Airports
    Task<List<AirportViewModel>> GetAirportsAsync();
    Task<AirportViewModel?> GetAirportAsync(Guid id);
    Task<AirportViewModel> CreateAirportAsync(CreateAirportViewModel model);
    Task<AirportViewModel> UpdateAirportAsync(Guid id, CreateAirportViewModel model);
    
    // Aircrafts
    Task<List<AircraftViewModel>> GetAircraftsAsync();
    Task<AircraftViewModel?> GetAircraftAsync(Guid id);
    Task<AircraftViewModel> CreateAircraftAsync(CreateAircraftViewModel model);
    
    // Flights
    Task<List<FlightViewModel>> GetFlightsAsync();
    Task<FlightViewModel?> GetFlightAsync(Guid id);
    Task<FlightViewModel> CreateFlightAsync(CreateFlightViewModel model);
    Task DeleteFlightAsync(Guid id);
    
    // Reports
    Task<FlightReportViewModel> GetFlightReportAsync();
}

