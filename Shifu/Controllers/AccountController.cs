using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shifu.Models;

namespace Shifu.Controllers;

// Created  by Laiba Ahmed 991691793
public class AccountController : Controller
{
    private readonly AppDbContext _db;
    public AccountController(AppDbContext db)
    {
        _db = db;
    }
    
    [HttpGet]
    public IActionResult SignUp() => View();

    [HttpPost]
    public async Task<IActionResult> SignUp(UserData model, string role)
    {
        if (!ModelState.IsValid) 
            return View(model);
        
        model.IsMentor = (role == "mentor");
        model.IsMentor = false;
        model.IsAvailable = false;
        model.IsAdmin = false;
        
        _db.Users.Add(model);
        await _db.SaveChangesAsync();
        
        return RedirectToAction("Login");
    }
    
    [HttpGet]
    public IActionResult Login() => View();


    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid credentials.");
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email ?? $"{user.FirstName} {user.LastName}"), 
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : user.IsMentor ? "Mentor" : "User")

        };
        
        if (user.IsMentor) claims.Add(new Claim(ClaimTypes.Role, "Mentor"));
        if (user.IsAdmin) claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        
        return RedirectToAction("Dashboard", "Home");
    }

    [HttpPost]

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
    
    
}