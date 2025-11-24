using Microsoft.EntityFrameworkCore;
using Shifu.Models;

namespace Shifu.Services
{
    public class JournalManager
    {
        private readonly AppDbContext _context;
        public JournalManager(AppDbContext context) => _context = context;

        public async Task<List<JournalEntry>> GetUserEntriesAsync(int userId)
            => await _context.JournalEntries
                .Where(j => j.UserId == userId)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

        public async Task AddEntryAsync(JournalEntry entry)
        {
            _context.JournalEntries.Add(entry);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetJournalStreakAsync(int userId)
        {
            var entries = await _context.JournalEntries
                                        .Where(j => j.UserId == userId)
                                        .OrderByDescending(j => j.CreatedAt)
                                        .ToListAsync();

            if (!entries.Any()) return 0;

            int streak = 1;
            var previousDate = entries[0].CreatedAt.Date;

            for (int i = 1; i < entries.Count; i++)
            {
                var diff = (previousDate - entries[i].CreatedAt.Date).Days;

                // consecutive day
                if (diff == 1) 
                {
                    streak++;
                    previousDate = entries[i].CreatedAt.Date;
                }
                // same day
                else if (diff == 0)
                {
                    
                    continue;
                }
                //streak broken
                else
                {
                    break; 
                }
            }

            return streak;
        }

    }
}
