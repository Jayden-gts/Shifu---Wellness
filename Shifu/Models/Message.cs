namespace Shifu.Models;
using Shifu.Models; 
using System.ComponentModel.DataAnnotations;

public class Message
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; } // Foreign Key to UserData 
    public bool SentByMentor  { get; set; }
    
    //[Required]
    public string Content { get; set; }

	public DateTime Timestamp { get; set; }

}