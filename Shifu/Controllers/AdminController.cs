using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shifu.Models;

namespace Shifu.Controllers;

// Created by Laiba Ahmed 991691793

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
        
        user.IsMentorApplicant = false;
        user.IsMentor = true;
        user.MentorStatus = "Approved"; 
        user.IsAvailable = true;
        
        await _db.SaveChangesAsync();

        Console.WriteLine($"User after approval: Id={user.Id}, IsMentor={user.IsMentor}, MentorStatus={user.MentorStatus}");

        // Log current HTTP context user claims
        var currentUser = HttpContext.User;
        Console.WriteLine($"Current HTTP User: Name={currentUser.Identity.Name}, IsAuthenticated={currentUser.Identity.IsAuthenticated}");
        foreach (var claim in currentUser.Claims)
        {
            Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
        }
        
        // Sign in the user immediately as a mentor
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim(ClaimTypes.Role, "Mentor")
        };

        var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(identity));

        // Redirect directly to mentor dashboard
        return RedirectToAction("Login", "Home");

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