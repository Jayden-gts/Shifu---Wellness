using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shifu.Models;

namespace Shifu.Controllers;

[Authorize (Roles = "Admin")]
public class AdminController : Controller 
{
    private readonly AppDbContext _db;
    public AdminController(AppDbContext db)
    {
        _db = db;
    }
    

    public IActionResult PendingMentors()
    {
        var pending = _db.Users.Where(u => u.IsMentorApplicant == true && !u.IsMentor).ToList();
        return View(pending);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveMentor(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return NotFound();
        user.IsMentorApplicant = true;
        user.IsAvailable = true;
        await _db.SaveChangesAsync();
        return Ok();
    }
    
    
    [HttpPost]
    public async Task<IActionResult> RejectMentor(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null)
            return NotFound();
        user.IsMentorApplicant = false;
        await _db.SaveChangesAsync();
        return Ok();
    }

    
}