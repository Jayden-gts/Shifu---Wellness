using Microsoft.AspNetCore.Mvc;
using Shifu.Models;
using Shifu.Services;

namespace Shifu.Controllers;
// Created by Jayden Seto - 991746683
public class EventsController : Controller
{

    private readonly OakvilleCityEventScraper _oakvilleScraper;
    private readonly MississaugaCityEventScraper _mississaugaCityEventScraper;

    public EventsController(OakvilleCityEventScraper oakvilleScraper, MississaugaCityEventScraper mississaugaCityEventScraper)
    {
        _oakvilleScraper = oakvilleScraper;
        _mississaugaCityEventScraper = mississaugaCityEventScraper;
    }
    
    // GET
    public async Task<IActionResult> Index()
    {
        var events = new List<CityEvent>();

        events.AddRange(await _oakvilleScraper.ScrapeEventsAsync());
        events.AddRange(await _mississaugaCityEventScraper.ScrapeEventsAsync());

       
        events = events.OrderBy(e => e.StartDate).ToList();

        return View(events);
    }
    
}