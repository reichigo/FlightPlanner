using FlightPlanner.Web.Exceptions;
using FlightPlanner.Web.Models;
using FlightPlanner.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightPlanner.Web.Controllers;

public class AircraftsController : Controller
{
    private readonly IFlightPlannerApiService _apiService;

    public AircraftsController(IFlightPlannerApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var aircrafts = await _apiService.GetAircraftsAsync();
        return View(aircrafts);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAircraftViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _apiService.CreateAircraftAsync(model);
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
        var aircraft = await _apiService.GetAircraftAsync(id);
        if (aircraft == null)
        {
            return NotFound();
        }
        return View(aircraft);
    }
}

