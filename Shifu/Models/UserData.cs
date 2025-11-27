using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shifu.Models;

//Created by Jayden Seto - 991746683
// This is a Model for a User entity with required fields and validation.

public class UserData
{
    [Key] public int Id { get; set; }

    [Required(ErrorMessage ="Please Enter your first name")]
    public string? FirstName { get; set; }
    
    [Required (ErrorMessage = "Please enter your last name")]
    public string? LastName { get; set; }
        
    [Required(ErrorMessage = "Please Enter your email")]
    [EmailAddress(ErrorMessage = "Invalid Email Format")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Please Enter your phone number")]
    [RegularExpression(@"\d{3}-\d{3}-\d{4}", ErrorMessage = "Phone number must be in the format 999-999-9999")]
    public string? PhoneNumber { get; set; }
    
    [Required (ErrorMessage = "Please Enter your password")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
    
    [Required (ErrorMessage = "Please Enter your password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string? PasswordConfirm { get; set; }

    public string? CurrentGoal { get; set; }

    public int? JournalStreak { get; set; }

    public int? GoalsCompleted { get; set; }
    
    [Required(ErrorMessage = "Please choose an option")]
    public bool? IsMentor { get; set; }

    [NotMapped]
    public List<JournalEntry> JournalEntries { get; set; } = new();

}