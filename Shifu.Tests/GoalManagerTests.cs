using Xunit;
using Microsoft.EntityFrameworkCore;
using Shifu.Models;
using Shifu.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shifu.Tests;

public class GoalManagerTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddGoalAsync_AddsGoal()
    {
        var context = GetInMemoryDb();
        var manager = new GoalManager(context);

        var goal = new Goal { UserId = 1, Title = "Test Goal", Description = "Test" };
        await manager.AddGoalAsync(goal);

        var goals = await manager.GetUserGoalsAsync(1);
        Assert.Single(goals);
        Assert.Equal("Test Goal", goals.First().Title);
    }
}