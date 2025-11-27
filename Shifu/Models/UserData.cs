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

    // mentor status 
    
    
    
    [Required(ErrorMessage = "Please choose an option")]
    public bool? IsMentorApplicant { get; set; } = null; // applied at signup
    public bool IsMentor { get; set; } = false; // set trye when admin approves 
    public bool IsAvailable { get; set; } = false; // mentor toggles when ready 
    public bool IsAdmin { get; set; } = false; // simple admin flag ****
    
    public string? MentorStatus { get; set; } // pending or approved 
    
    
    // basic profile used on mentor list 
    public string? Bio { get; set; }
    public string? Qualifications { get; set; }
    public string? Specialities { get; set; }
    
    
    public string? CurrentGoal { get; set; }

    public int? JournalStreak { get; set; }

    public int? GoalsCompleted { get; set; }
    

    [NotMapped]
    public List<JournalEntry> JournalEntries { get; set; } = new();

}