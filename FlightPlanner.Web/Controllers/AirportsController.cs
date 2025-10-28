using FlightPlanner.Web.Exceptions;
using FlightPlanner.Web.Models;
using FlightPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Web.Controllers;

public class AirportsController : Controller
{
    private readonly IFlightPlannerApiService _apiService;

    public AirportsController(IFlightPlannerApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var airports = await _apiService.GetAirportsAsync();
        return View(airports);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAirportViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _apiService.CreateAirportAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var airport = await _apiService.GetAirportAsync(id);
        if (airport == null)
        {
            return NotFound();
        }
        return View(airport);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var airport = await _apiService.GetAirportAsync(id);
        if (airport == null)
        {
            return NotFound();
        }

        var model = new CreateAirportViewModel
        {
            Iata = airport.Iata,
            Name = airport.Name,
            Latitude = airport.Latitude,
            Longitude = airport.Longitude
        };

        ViewData["AirportId"] = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CreateAirportViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["AirportId"] = id;
            return View(model);
        }

        try
        {
            await _apiService.UpdateAirportAsync(id, model);
            return RedirectToAction(nameof(Index));
        }
        catch (ApiException ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewData["AirportId"] = id;
            return View(model);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
            ViewData["AirportId"] = id;
            return View(model);
        }
    }
}

