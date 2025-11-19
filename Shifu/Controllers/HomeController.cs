using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shifu.Models;

namespace Shifu.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserDataRepository _repository;


    public HomeController(ILogger<HomeController> logger, UserDataRepository repository)
    {
        _logger = logger;
        _repository = repository;
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
    
    private static UserData? LoggedInUser;
    
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
    public IActionResult Dashboard()
    {
        if (LoggedInUser == null)
            return RedirectToAction("Login");

        return View(LoggedInUser);
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