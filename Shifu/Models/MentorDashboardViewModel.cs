using System.Collections.Generic;
using Shifu.Models;

namespace Shifu.Models;

// Created by Laiba Ahmed 991691793

public class MentorDashboardViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<UserData> Users { get; set; }

}