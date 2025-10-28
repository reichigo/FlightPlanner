namespace FlightPlanner.Web.Models;

public class AircraftViewModel
{
    public Guid Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public double CruiseSpeedKts { get; set; }
    public double FuelBurnPerHourKg { get; set; }
    public double TakeoffFuelKg { get; set; }
}

public class CreateAircraftViewModel
{
    public string Model { get; set; } = string.Empty;
    public double CruiseSpeedKts { get; set; }
    public double FuelBurnPerHourKg { get; set; }
    public double TakeoffFuelKg { get; set; }
}

