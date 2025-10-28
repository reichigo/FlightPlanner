namespace FlightPlanner.Web.Models;

public class AirportViewModel
{
    public Guid Id { get; set; }
    public string Iata { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class CreateAirportViewModel
{
    public string Iata { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

