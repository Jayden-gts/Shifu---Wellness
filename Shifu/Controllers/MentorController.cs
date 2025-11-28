using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Shifu.Hubs;

namespace Shifu.Controllers;

using Microsoft.AspNetCore.Mvc;
using Shifu.Models;
using Shifu.Services;
using System.Linq;
using System.Threading.Tasks;

// Created by Laiba Ahmed 991691793

[Authorize(Roles = "Mentor")]
public class MentorController : Controller
{
    private readonly AppDbContext _db; 
    private readonly MentorService  _service;
    private readonly IHubContext<MentorUserHub> _hubContext; // check this 

    public MentorController(AppDbContext db, IHubContext<MentorUserHub> hubContext)
    {
        _db = db;
        _service = new MentorService(db);
        _hubContext = hubContext;

    }
    
    [HttpPost]
    public async Task<IActionResult> AwardBadge([FromBody] BadgeDto dto)
    {
        var badge = new Badge
        {
            AwardedToId = dto.UserId.ToString(),
            AwardedById = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Name = dto.BadgeName,
            ImageUrl = dto.ImageUrl,
            AwardedOn = DateTime.UtcNow
        };

        _db.Badges.Add(badge);
        await _db.SaveChangesAsync();
        return Ok();
    }

    public class BadgeDto
    {
        public int UserId { get; set; }
        public string BadgeName { get; set; }
        public string ImageUrl { get; set; }
    }



    public async Task<IActionResult> Chat()
    {
        var mentorId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
        var users = await _service.GetUsersAssignedToMentor(mentorId);
        return View(users);
    }
    
    [Authorize(Roles = "Mentor")]
    [HttpGet("Mentor/MentorDashboard")]
    public async Task<IActionResult> MentorDashboard()
    { 
        var mentorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        
       


        // Get assigned users asynchronously
        var users = await _service.GetUsersAssignedToMentor(mentorId);

        var model = new MentorDashboardViewModel
        {
            Users = users
        };
    
        return View(model);
    }


    [HttpGet]
    public async Task<IActionResult> LoadMessages(int userId)
    {
        var messsages = await _service.GetMessagesForUser(userId);
        return Json(messsages);
    }

    // mentor sends message to user 
    [HttpPost]
    public async Task<IActionResult> SendMessages(int userId, string message)
    {
        var mentorId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

        var assign = await _db.UserMentorAssignments.FirstOrDefaultAsync(a => a.UserId == userId && a.MentorId == mentorId);
        if (assign == null)
            return Forbid();

        // saves the message in the database 
        await _service.SaveMessage(userId, message, sentByMentor: true, mentorId: mentorId);
        
        // this broadcast to all the clients using SignalR 
        await  _hubContext.Clients.Group($"user{userId}").SendAsync("RecieveMessage", userId, message, true, mentorId);
        return Ok();
    }

}