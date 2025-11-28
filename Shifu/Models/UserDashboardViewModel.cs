using Shifu.Models;
using System.Collections.Generic;

namespace Shifu.Models;

// Created by Laiba Ahmed 991691793

public class UserDashboardViewModel
{
        public List<UserData> Mentors { get; set; }
        public int? AssignedMentorId { get; set; }
        public List<Badge> Badges { get; set; }
    
}