namespace Shifu.Models;

// Created by Laiba Ahmed 991691793

public class Badge
{
    public int Id { get; set; }
    public string Name { get; set; }           // Label for badge
    public string ImageUrl { get; set; }       // Badge image path
    public DateTime AwardedOn { get; set; }
    public string AwardedById { get; set; }    // Mentor ID
    public string AwardedToId { get; set; }    // Student ID
    
}