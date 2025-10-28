using FlightPlanner.Application.Dto.Request;
using FlightPlanner.Application.Dto.Response;
using FlightPlanner.Domain.Entities;

namespace FlightPlanner.Application.Mappers;

public static class AircraftMapper
{
    public static Aircraft ToDomain(this CreateAircraftRequest request)
    {
        return new Aircraft(
            request.Model,
            request.CruiseSpeedKts,
            request.FuelBurnPerHourKg,
            request.TakeoffFuelKg
        );
    }

    public static AircraftResponse ToResponse(this Aircraft aircraft)
    {
        return new AircraftResponse(
            aircraft.Id,
            aircraft.Model,
            aircraft.CruiseSpeedKts,
            aircraft.FuelBurnPerHourKg,
            aircraft.TakeoffFuelKg
        );
    }
}

