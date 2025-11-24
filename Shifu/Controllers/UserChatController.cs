using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Shifu.Hubs;
using Shifu.Models;
using Shifu.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Shifu.Controllers;

public class UserChatController : Controller
{
    private readonly AppDbContext _db; 
    private readonly MentorService  _service;
    private readonly IHubContext<UserChatHub> _hubContext; // check this 

    public UserChatController(AppDbContext db, IHubContext<UserChatHub> hubContext)
    {
        _db = db;
        _service = new MentorService(db);
        _hubContext = hubContext;
    }

    public IActionResult Chat()
    {
        //var users = _db.Users.ToList();
        return View();
    }


    [HttpGet]
    public async Task<IActionResult> LoadMessages(int mentorId)
    {
        var messsages = await _service.GetMessagesForUser(mentorId);
        return Json(messsages);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessages(int mentorId, string message)
    {
        // saves the message in the database 
        await _service.SaveMessage(mentorId, message);
        
        // this broadcast to all the clients using SignalR 
        await  _hubContext.Clients.All.SendAsync("ReceiveMessage", mentorId, message);
        return Ok();
    }

    
    
    
}