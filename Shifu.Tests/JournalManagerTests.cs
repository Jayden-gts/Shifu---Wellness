namespace Shifu.Tests;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Shifu.Models;
using Shifu.Services;
using System;
using System.Threading.Tasks;
using System.Linq;

public class JournalManagerTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddEntryAsync_AddsEntrySuccessfully()
    {
       
        var context = GetInMemoryDb();
        var manager = new JournalManager(context);

        var entry = new JournalEntry
        {
            UserId = 1,
            Content = "Test journal entry",
            Footnote = "#goal:TestGoal",
            CreatedAt = DateTime.Now
        };

     
        await manager.AddEntryAsync(entry);
        var entries = await manager.GetUserEntriesAsync(1);

      
        Assert.Single(entries);
        Assert.Equal("Test journal entry", entries.First().Content);
    }

    [Fact]
    public async Task GetJournalStreakAsync_CalculatesCorrectStreak()
    {
       
        var context = GetInMemoryDb();
        var manager = new JournalManager(context);

        var now = DateTime.Now;
        await manager.AddEntryAsync(new JournalEntry { UserId = 1, Content = "Day 3", CreatedAt = now });
        await manager.AddEntryAsync(new JournalEntry { UserId = 1, Content = "Day 2", CreatedAt = now.AddDays(-1) });
        await manager.AddEntryAsync(new JournalEntry { UserId = 1, Content = "Day 1", CreatedAt = now.AddDays(-2) });

        
        var streak = await manager.GetJournalStreakAsync(1);

        
        Assert.Equal(3, streak);
    }
}




