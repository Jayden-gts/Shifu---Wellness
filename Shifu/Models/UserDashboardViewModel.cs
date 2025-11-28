using Shifu.Models;
using System.Collections.Generic;

namespace Shifu.Models;

public class UserDashboardViewModel
{
        public List<UserData> Mentors { get; set; }
        public int? AssignedMentorId { get; set; }
        public List<Badge> Badges { get; set; }
    
}