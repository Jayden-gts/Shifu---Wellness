namespace Shifu.Models;
using Shifu.Models; 
using System.ComponentModel.DataAnnotations;

// Created by Laiba Ahmed 991691793

public class Message
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; } // Foreign Key to UserData 
    
    public int? MentorId { get; set; }
    public bool SentByMentor  { get; set; }

    [Required] public string Content { get; set; } = ""; 

	public DateTime Timestamp { get; set; } = DateTime.UtcNow;

}