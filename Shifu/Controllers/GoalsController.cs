using Microsoft.AspNetCore.Mvc;
using Shifu.Models;
using Shifu.Services;

namespace Shifu.Controllers
{
    public class GoalsController : Controller
    {
        private readonly GoalManager _goalManager;
        private readonly UserData? _currentUser = HomeController.LoggedInUser;

        public GoalsController(GoalManager goalManager) => _goalManager = goalManager;

        public async Task<IActionResult> Index()
        {
            if (_currentUser == null) return RedirectToAction("Login", "Home");
            var goals = await _goalManager.GetUserGoalsAsync(_currentUser.Id);
            return View(goals);
        }

        [HttpPost]
        public async Task<IActionResult> AddGoal(string title, string? description, DateTime? targetDate)
        {
            if (_currentUser == null) return RedirectToAction("Login", "Home");

            await _goalManager.AddGoalAsync(new Goal
            {
                UserId = _currentUser.Id,
                Title = title,
                Description = description,
                TargetDate = targetDate
            });

            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> MarkCompleted(int id)
        {
            var goal = (await _goalManager.GetUserGoalsAsync(_currentUser.Id)).FirstOrDefault(g => g.Id == id);
            if (goal != null)
            {
                goal.Completed = true;
                await _goalManager.UpdateGoalAsync(goal);
            }
            return RedirectToAction("Index");
        }


    }
}
