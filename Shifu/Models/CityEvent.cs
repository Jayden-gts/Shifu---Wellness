namespace Shifu.Models;
// Created by Jayden Seto - 991746683
// This is a Model for a local city event
public class CityEvent
{
    public string Title { get; set; }
    public string Location { get; set; }
    public DateTime? StartDate  { get; set; }
    public string EndTime { get; set; }
    public string Url { get; set; }
    public string ImageUrl { get; set; }
    public string City { get; set; }
}

