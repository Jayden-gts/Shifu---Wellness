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

    public MentorController(AppDbContext db)
    {
        _db = db;
        _service = new MentorService(db);
    }

    public IActionResult Chat()
    {
        var users = _db.Users.ToList();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> LoadMessages(int userId)
    {
        var messages = await _service.GetMessagesForUser(userId);
        return Json(messages);
    }

}