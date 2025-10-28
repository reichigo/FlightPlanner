namespace FlightPlanner.Web.Models;

public class FlightViewModel
{
    public Guid Id { get; set; }
    public AirportViewModel Origin { get; set; } = new();
    public AirportViewModel Destination { get; set; } = new();
    public AircraftViewModel Aircraft { get; set; } = new();
    public DateTime? DepartureAt { get; set; }
    public FlightEstimatesViewModel? Estimates { get; set; }
}

public class FlightEstimatesViewModel
{
    public double DistanceKm { get; set; }
    public TimeSpan Duration { get; set; }
    public double FuelKg { get; set; }
}

public class CreateFlightViewModel
{
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Origin airport is required")]
    public Guid OriginAirportId { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Destination airport is required")]
    public Guid DestinationAirportId { get; set; }

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Aircraft is required")]
    public Guid AircraftId { get; set; }

    public DateTime? DepartureAt { get; set; }
}

public class FlightReportViewModel
{
    public List<FlightViewModel> Flights { get; set; } = new();
    public int TotalFlights { get; set; }
    public double TotalDistanceKm { get; set; }
    public double TotalFuelKg { get; set; }
}

