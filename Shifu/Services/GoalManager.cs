using Microsoft.EntityFrameworkCore;
using Shifu.Models;

namespace Shifu.Services
{
    // created by Jonathan Ghattas  #991703952
    public class GoalManager
    {
        private readonly AppDbContext _context;
        public GoalManager(AppDbContext context) => _context = context;

        public async Task<List<Goal>> GetUserGoalsAsync(int userId)
        => await _context.Goals.Where(g => g.UserId == userId).ToListAsync();

    public async Task AddGoalAsync(Goal goal)
    {
        _context.Goals.Add(goal);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateGoalAsync(Goal goal)
    {
        _context.Goals.Update(goal);
        await _context.SaveChangesAsync();
    }
    }
}
