using Microsoft.AspNetCore.Mvc;
using Shifu.Models;
using Shifu.Services;
using System.Diagnostics;

namespace Shifu.Controllers;

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
        
        await _repository.AddUserDataAsync(user);
        return RedirectToAction("Login");
        

    }
    
    [HttpGet]
    public IActionResult Login()
    {
        return View(); 
    }

    
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {

        var user = await _repository.GetUserAsync(email, password);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password");
            return View("Login");
        }

        LoggedInUser = user;
        return RedirectToAction("Dashboard");
    }

    [HttpGet]
    public IActionResult SignOut()
    {
        LoggedInUser = null;
        return RedirectToAction("Login");
    }
    
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


        // this is to track the "age" of a goal in order to be able to send the oldest goal to the dashboard via ViewBag
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
}