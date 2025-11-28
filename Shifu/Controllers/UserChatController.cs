using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shifu.Hubs;
using Shifu.Models;
using Shifu.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Shifu.Controllers;

[Authorize]
public class UserChatController : Controller
{
    private readonly AppDbContext _db; 
    private readonly MentorService  _service;
    private readonly IHubContext<MentorUserHub> _hubContext; // check this 

    public UserChatController(AppDbContext db, IHubContext<MentorUserHub> hubContext)
    {
        _db = db;
        _service = new MentorService(db);
        _hubContext = hubContext;
    }
    
    // show the avaliable mentors with the qualifications 
    public async Task<IActionResult> Chat()
    {
        //var mentors = await _service.GetMentorsAvailable();
        //return View(mentors);
        
        
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var assigned = await _service.GetAssignmentForUser(userId);
        int? assignedMentorId = assigned?.MentorId;

        List<UserData> mentors;
        if (assignedMentorId != null)
            mentors = await _service.GetMentorsAllApproved(); // or just the assigned one
        else
            mentors = await _service.GetMentorsAvailable();

        ViewBag.AssignedMentorId = assignedMentorId;
        return View(mentors);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAssignedMentor(int userId)
    {
        var assigned = await _service.GetAssignmentForUser(userId);
        return Json(new { mentorId = assigned?.MentorId });
    }

    
    
    [HttpPost]
    public async Task<IActionResult> AssignMentor(int mentorId)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
        
        var existing = await _db.UserMentorAssignments.FirstOrDefaultAsync(a => a.UserId == userId);
        if (existing != null)
            return BadRequest("Already assigned");
        
        await _service.AssignMentor(userId, mentorId);
        return Ok();
    }

    //public IActionResult Chat()
    //{
        //var users = _db.Users.ToList();
        //return View();
    //}


    [HttpGet]
    public async Task<IActionResult> LoadMessages(int userId)
    {
        var callerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
        if (callerId != userId)
            return Forbid(); 
        
        var messsages = await _service.GetMessagesForUser(userId);
        return Json(messsages);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessages(int mentorId, string message)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
        
        var assign = await _db.UserMentorAssignments.FirstOrDefaultAsync(a => a.UserId == userId && a.MentorId == mentorId);
        if (assign == null)
            return Forbid();
        
        
        // saves the message in the database 
        await _service.SaveMessage(userId, message, sentByMentor: false, mentorId: mentorId);
        
        // this broadcast to all the clients using SignalR 
        await  _hubContext.Clients.Group($"user-{userId}").SendAsync("RecieveMessage", userId, message, false, mentorId);
        return Ok();
    }
}