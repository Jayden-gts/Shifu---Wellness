using System;
using System.ComponentModel.DataAnnotations;

namespace Shifu.Models
{
    public class Goal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public UserData User { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }
        public bool Completed { get; set; } = false;
        public DateTime? TargetDate { get; set; }



    }
}
