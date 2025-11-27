using System.Globalization;
using HtmlAgilityPack;
using Shifu.Models;

namespace Shifu.Services;
//Created by Jayden Seto - 991746683
// This is a web scraping service for the events on the city of Oakville website
public class OakvilleCityEventScraper
{
    private readonly HttpClient _httpClient;

    public OakvilleCityEventScraper(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    //Scrapes event data from website
    //Uses HTMLAgilityPack for parsing raw html
    public async Task<List<CityEvent>> ScrapeEventsAsync()
    {
        var events = new List<CityEvent>();

        var url = "https://www.oakville.ca/community-events/events/events-calendar/";
        var html = await _httpClient.GetStringAsync(url);
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        //Selecting all event card containers
        var eventNodes =
            doc.DocumentNode.SelectNodes(
                "//ul[contains(@class,'featured-events-container')]/li[contains(@class,'feature-item')]");
        // For each of them, extract individual html nodes
        foreach (var node in eventNodes)
        {
            var titleNode = node.SelectSingleNode(".//div[contains(@class, 'feature-item-text-title')]");
            var dayNode = node.SelectSingleNode(".//div[contains(@class,'start-date')]/div[contains(@class,'day')]");
            var monthNode = node.SelectSingleNode(".//div[contains(@class,'start-date')]/div[contains(@class,'month')]");
            var endTimeNode = node.SelectSingleNode(".//div[contains(@class,'end-event')]");
            var locationNode = node.SelectSingleNode(".//div[contains(@class,'feature-item-text-description')]");
            var linkNode = node.SelectSingleNode(".//a[contains(@class,'feature-item-image-link')]");
            var imageNode = node.SelectSingleNode(".//img");
            
            //Extract text from the nodes
            string title = titleNode?.InnerText.Trim() ?? "No title";
            string day = dayNode?.InnerText.Trim() ?? "";
            string month = monthNode?.InnerText.Trim() ?? "";
            string endTime = endTimeNode?.InnerText.Trim() ?? "";
            string location = locationNode?.InnerText.Trim() ?? "";
            string link = linkNode?.GetAttributeValue("href", "#") ?? "#";
            string imageUrl = imageNode?.GetAttributeValue("src", "") ?? "";
            //Date/Time parsing and formatting
            DateTime? startDate = null;
            if (!string.IsNullOrEmpty(day) && !string.IsNullOrEmpty(month))
            {
                if (DateTime.TryParseExact($"{day} {month} 2025", "d MMM yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out var dt))
                {
                    startDate = dt;
                }
            }
            //Add new event using parsed data.
            events.Add(new CityEvent
            {
                Title = title,
                Location = location,
                StartDate = startDate,
                EndTime = endTime,
                Url = "https://www.oakville.ca" + link,
                City = "Oakville",
                ImageUrl = imageUrl
                
            });

        }

        return events;
    }
}

