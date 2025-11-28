using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shifu.Models;

namespace Shifu.Controllers;

// Created by Laiba Ahmed 991691793

[Authorize]
public class MentorApplicationController : Controller 
{
    private readonly AppDbContext _db;
    private readonly ILogger<MentorApplicationController> _logger;

    public MentorApplicationController(AppDbContext db, ILogger<MentorApplicationController> logger)
    {
        _db = db;
        _logger = logger;
        
    }
    
    public IActionResult Apply() => View();

    public IActionResult Pending()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Apply(MentorApplicationViewModel vm)
    {
        _logger.LogInformation("Apply POST called");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid: {@ModelState}", ModelState);
            return View(vm);
        }

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            _logger.LogWarning("User is not logged in, redirecting to login");
            return RedirectToAction("Login", "Home");
        }

        int userId = int.Parse(userIdClaim.Value);
        _logger.LogInformation("Current user ID: {UserId}", userId);

        var user = await _db.Users.FindAsync(userId);
        if (user == null)
        {
            _logger.LogError("User not found in DB for ID {UserId}", userId);
            return NotFound();
        }

        try
        {
            user.IsMentorApplicant = true;
            user.MentorStatus = "Pending";
            user.Bio = vm.Bio;
            user.Qualifications = vm.Qualifications;
            user.Specialities = vm.Specialities;

            await _db.SaveChangesAsync();
            _logger.LogInformation("Mentor application saved for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving mentor application for user {UserId}", userId);
            throw;
        }

        return RedirectToAction("Pending", "MentorApplication");
    }
}
    


public class MentorApplicationViewModel
{
    public string? Bio { get; set; }
    public string? Qualifications { get; set; }
    public string? Specialities { get; set; }
}

