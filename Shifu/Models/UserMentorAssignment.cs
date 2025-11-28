using System.ComponentModel.DataAnnotations;

namespace Shifu.Models;

// Created by Laiba Ahmed 991691793
public class UserMentorAssignment
{
    [Key]
    public int Id { get; set; }
    
    // who is the mentee 
    public int UserId { get; set; }
    
    // which mentor 
    public int MentorId { get; set; }
    
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
}