using System.Text.Json;
using Shifu.Models;

namespace Shifu.Services;

using System.Globalization;
using HtmlAgilityPack;
using PuppeteerSharp;

public class MississaugaCityEventScraper
{
    public MississaugaCityEventScraper()
    {
        
    }
    public async Task<List<CityEvent>> ScrapeEventsAsync()
    {
        var events = new List<CityEvent>();

       
        await new BrowserFetcher().DownloadAsync();

        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        
        await page.GoToAsync("https://www.mississauga.ca/events-and-attractions/events-calendar/#/",
            WaitUntilNavigation.Networkidle0);

       
        await page.WaitForSelectorAsync("li.event-card");

        
        
        var eventsData = await page.EvaluateExpressionAsync<string>(@"
            Array.from(document.querySelectorAll('li.event-card')).map(e => {
                const title = e.querySelector('.event-title')?.innerText || '';
                const timePlace = e.querySelector('.time-and-place')?.innerText || '';
                const link = e.querySelector('a')?.href || '';
                const img = e.querySelector('img')?.src || '';
                return JSON.stringify({ title, timePlace, link, img });
            }).join('|');
        ");

       
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

