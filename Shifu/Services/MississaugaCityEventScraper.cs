using System.Text.Json;
using Shifu.Models;

namespace Shifu.Services;

using System.Globalization;
using HtmlAgilityPack;
using PuppeteerSharp;
//Created By Jayden Seto - 991746683
// This is a service for web scraping from the City of Mississauga's events website.
public class MississaugaCityEventScraper
{
    public MississaugaCityEventScraper()
    {
        
    }
    //Scrapes events calendar.
    //Launches a headless browser instance using PuppeteerSharp
    //Extracts data dynamically from HTML list items on the website
    //Parses into structured event fields
    public async Task<List<CityEvent>> ScrapeEventsAsync()
    {
        var events = new List<CityEvent>();

       
        await new BrowserFetcher().DownloadAsync();

        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        
        await page.GoToAsync("https://www.mississauga.ca/events-and-attractions/events-calendar/#/",
            WaitUntilNavigation.Networkidle0);

       
        await page.WaitForSelectorAsync("li.event-card");

        
        // uses JavaScript in the page to extract the data for the event card details
        var eventsData = await page.EvaluateExpressionAsync<string>(@"
            Array.from(document.querySelectorAll('li.event-card')).map(e => {
                const title = e.querySelector('.event-title')?.innerText || '';
                const timePlace = e.querySelector('.time-and-place')?.innerText || '';
                const link = e.querySelector('a')?.href || '';
                const img = e.querySelector('img')?.src || '';
                return JSON.stringify({ title, timePlace, link, img });
            }).join('|');
        ");

       //Creates event from JSON data
        events = eventsData
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(ev => JsonSerializer.Deserialize<TempEvent>(ev)!)
            .Select(te => new CityEvent
            {
                Title = te.title,
                EndTime = te.timePlace.Split(" - ")[0].Trim(),
                Location = te.timePlace.Contains(" - ") ? te.timePlace.Split(" - ")[1].Trim() : "",
                Url = te.link,
                ImageUrl = te.img,
                City = "Mississauga"
            })
            .ToList();

        return events;
    }
}

