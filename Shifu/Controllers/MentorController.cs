using Microsoft.AspNetCore.SignalR;
using Shifu.Hubs;

namespace Shifu.Controllers;

using Microsoft.AspNetCore.Mvc;
using Shifu.Models;
using Shifu.Services;
using System.Linq;
using System.Threading.Tasks;

public class MentorController : Controller
{
    private readonly AppDbContext _db; 
    private readonly MentorService  _service;
    private readonly IHubContext<MentorChatHub> _hubContext; // check this 

    public MentorController(AppDbContext db, IHubContext<MentorChatHub> hubContext)
    {
        _db = db;
        _service = new MentorService(db);
        _hubContext = hubContext;
    }

    public IActionResult Chat()
    {
        var users = _db.Users.ToList();
        return View(users);
    }


    [HttpGet]
    public async Task<IActionResult> LoadMessages(int userId)
    {
        var messsages = await _service.GetMessagesForUser(userId);
        return Json(messsages);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessages(int userId, string message)
    {
        // saves the message in the database 
        await _service.SaveMessage(userId, message);
        
        // this broadcast to all the clients using SignalR 
        await  _hubContext.Clients.All.SendAsync("ReceiveMessage", userId, message);
        return Ok();
    }

}