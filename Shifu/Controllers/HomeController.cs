using Microsoft.AspNetCore.Mvc;
using Shifu.Models;
using Shifu.Services;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Shifu.Controllers;
// Created by Jayden Seto - 991746683 
// Controller for handling user authentication during sign up and log in, and profile management 


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserDataRepository _repository;
    private readonly JournalManager _journalManager;
    private readonly GoalManager _goalManager;
    private readonly UserData? _currentUser = HomeController.LoggedInUser;


    public HomeController(ILogger<HomeController> logger, UserDataRepository repository, JournalManager journalManager, GoalManager goal)
    {
        _logger = logger;
        _repository = repository;
        _journalManager = journalManager;
        _goalManager = goal;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult SignUp()
    {
        return View();
    }
    public static UserData? LoggedInUser;
    
    [HttpPost]
    public async Task<IActionResult> SignUp(UserData user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        if (await _repository.EmailExistsAsync(user.Email!))
        {
            string loginPath = "/Home/Login"; 
    
            string errorMessage = $"A user with this email address already exists. Do you want to <a href=\"{loginPath}\">login</a>?";
    
            ModelState.AddModelError("Email", errorMessage);
            return View(user);
        }
        
        // Ensure they picked a role
        if (user.IsMentor == null)
        {
            ModelState.AddModelError("IsMentorApplicant", "Please select a role: User or Mentor.");
            return View(user);
        }
        
        await _repository.AddUserDataAsync(user);
        
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, (user.IsMentorApplicant ?? false) ? "Mentor" : "User")
        };
        var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(identity));
        LoggedInUser = user;
        
        if (user.IsMentorApplicant == true)
        {
            user.MentorStatus = "Pending";
            await _repository.UpdateUserAsync(user); // Save the status to DB

            // Redirect to Mentor Application form
            return RedirectToAction("Apply", "MentorApplication");
        }
        
        LoggedInUser = user;
        return RedirectToAction("Login");
    }
    
    //Display Login Page
    [HttpGet]
    public IActionResult Login()
    {
        return View(); 
    }

    //Log in after validation, sets logged in user to current user.
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        
        // Hardcoded admin credentials
        if (email == "admin@gmail.com" && password == "AdminPass123")
        {
            var adminClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "0"), // fixed ID for admin
                new Claim(ClaimTypes.Name, "Admin User"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var adminIdentity = new ClaimsIdentity(adminClaims, "MyCookieAuth");
            await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(adminIdentity));

            // Redirect directly to admin dashboard
            return RedirectToAction("PendingMentors", "Admin");
        } 
        
        var user = await _repository.GetUserAsync(email, password);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password");
            return View("Login");
        }

        string role;
        if (user.IsAdmin)
            role = "Admin";
        else if (user.IsMentor)
            role = "Mentor";
        else 
            role = "User";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim(ClaimTypes.Role, role),

        };
        
        var identity = new ClaimsIdentity(claims, "MyCookieAuth");
        var principal = new ClaimsPrincipal(identity);
        
        await HttpContext.SignInAsync("MyCookieAuth", principal);

        LoggedInUser = user;
        
        // Redirect based on role and mentor status
        if (user.IsAdmin)
            return RedirectToAction("PendingMentors", "Admin");

        if (user.IsMentor && user.MentorStatus == "Approved")
            return RedirectToAction("MentorDashboard", "Mentor");

        if (user.IsMentorApplicant.GetValueOrDefault() && user.MentorStatus == "Pending")
            return RedirectToAction("PendingApplications", "Mentor");

        return RedirectToAction("Dashboard"); // Regular users
    }

    //Signs out user
    [HttpGet]
    public IActionResult SignOut()
    {
        LoggedInUser = null;
        return RedirectToAction("Login");
    }
    
    //Displays Dashboard for logged-in user, if currently no one is logged in, redirects to the log-in page
    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        if (LoggedInUser == null)
            return RedirectToAction("Login");

        var goals = await _goalManager.GetUserGoalsAsync(_currentUser.Id);

        // Find oldest pending goal (or null if none exist)
        var oldestGoal = goals
            .Where(g => !g.Completed)
            .OrderBy(g => g.TargetDate ?? DateTime.MaxValue) // Safely handles null target dates
            .FirstOrDefault();


        ViewBag.OldestGoal = oldestGoal;

        // load journal entries
        LoggedInUser.JournalEntries = await _journalManager.GetUserEntriesAsync(LoggedInUser.Id);
        // get journal streak
        LoggedInUser.JournalStreak = await _journalManager.GetJournalStreakAsync(LoggedInUser.Id);

        return View(LoggedInUser);
    }

    [HttpGet]
    public IActionResult EditProfile()
    {
        if (LoggedInUser == null)
            return RedirectToAction("Login");
        return View(LoggedInUser);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(UserData user)
    {
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        if (LoggedInUser == null)
            return View("Login");
        
        LoggedInUser.FirstName = user.FirstName;
        LoggedInUser.LastName = user.LastName;
        LoggedInUser.Email = user.Email;
        LoggedInUser.PhoneNumber = user.PhoneNumber;
        
        if (!string.IsNullOrWhiteSpace(user.Password))
        {
            if (user.Password != user.PasswordConfirm)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(user);
            }

            LoggedInUser.Password = user.Password;
        }
        
        await _repository.UpdateUserAsync(LoggedInUser);

        TempData["Success"] = "Profile updated!";
        return RedirectToAction("Dashboard");
    }

    
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    [Authorize(Roles="Mentor")]
    public IActionResult Pending() => View(); 

    [Authorize(Roles="Mentor")]
    public IActionResult Rejected() => View(); 

    
}