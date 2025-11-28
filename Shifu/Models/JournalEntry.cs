using System;
using System.ComponentModel.DataAnnotations;

namespace Shifu.Models
{
    // created by Jonathan Ghattas  #991703952
    public class JournalEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public UserData User { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? Mood { get; set; }

        public string? Footnote { get; set; }



    }
}
