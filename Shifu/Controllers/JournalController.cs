using Microsoft.AspNetCore.Mvc;
using Shifu.Models;
using Shifu.Services;

namespace Shifu.Controllers
{
    public class JournalController : Controller
    {
        private readonly JournalManager _journalManager;
        private readonly UserData? _currentUser = HomeController.LoggedInUser;

        public JournalController(JournalManager journalManager) => _journalManager = journalManager;

        [HttpPost]
        public async Task<IActionResult> AddEntry(int userId, string content, string? mood)
        {
            if (HomeController.LoggedInUser == null || HomeController.LoggedInUser.Id != userId)
                return Unauthorized();

            var entry = new JournalEntry
            {
                UserId = userId,
                Content = content,
                Mood = mood,
               
                CreatedAt = DateTime.Now
            };

            await _journalManager.AddEntryAsync(entry);

            HomeController.LoggedInUser.JournalEntries = await _journalManager.GetUserEntriesAsync(userId);


            HomeController.LoggedInUser.JournalStreak = await _journalManager.GetJournalStreakAsync(userId);

            return PartialView("_JournalPartial", HomeController.LoggedInUser);
        }
    }
}
