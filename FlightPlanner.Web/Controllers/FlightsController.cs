using FlightPlanner.Web.Exceptions;
using FlightPlanner.Web.Models;
using FlightPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Web.Controllers;

public class FlightsController : Controller
{
    private readonly IFlightPlannerApiService _apiService;

    public FlightsController(IFlightPlannerApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var flights = await _apiService.GetFlightsAsync();
        return View(flights);
    }

    public async Task<IActionResult> Create(Guid? aircraftId = null)
    {
        ViewBag.Airports = await _apiService.GetAirportsAsync();
        ViewBag.Aircrafts = await _apiService.GetAircraftsAsync();

        var model = new CreateFlightViewModel();
        if (aircraftId.HasValue)
        {
            model.AircraftId = aircraftId.Value;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateFlightViewModel model)
    {
        // Additional validation for Guid.Empty
        if (model.OriginAirportId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(model.OriginAirportId), "Please select an origin airport");
        }

        if (model.DestinationAirportId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(model.DestinationAirportId), "Please select a destination airport");
        }

        if (model.AircraftId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(model.AircraftId), "Please select an aircraft");
        }

        if (model.OriginAirportId == model.DestinationAirportId && model.OriginAirportId != Guid.Empty)
        {
            ModelState.AddModelError("", "Origin and destination airports must be different");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Airports = await _apiService.GetAirportsAsync();
            ViewBag.Aircrafts = await _apiService.GetAircraftsAsync();
            return View(model);
        }

        try
        {
            // DEBUG: Log the values being sent
            Console.WriteLine($"DEBUG - OriginAirportId: {model.OriginAirportId}");
            Console.WriteLine($"DEBUG - DestinationAirportId: {model.DestinationAirportId}");
            Console.WriteLine($"DEBUG - AircraftId: {model.AircraftId}");

            await _apiService.CreateFlightAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.Airports = await _apiService.GetAirportsAsync();
            ViewBag.Aircrafts = await _apiService.GetAircraftsAsync();
            return View(model);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
            ViewBag.Airports = await _apiService.GetAirportsAsync();
            ViewBag.Aircrafts = await _apiService.GetAircraftsAsync();
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var flight = await _apiService.GetFlightAsync(id);
        if (flight == null)
        {
            return NotFound();
        }
        return View(flight);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _apiService.DeleteFlightAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Unexpected error: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Report()
    {
        var report = await _apiService.GetFlightReportAsync();
        return View(report);
    }
}

