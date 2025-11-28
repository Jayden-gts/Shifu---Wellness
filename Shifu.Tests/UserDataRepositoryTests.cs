namespace Shifu.Tests;

using Microsoft.EntityFrameworkCore;
using Shifu.Models;
using System;
using System.Threading.Tasks;
using Xunit;

public class UserDataRepositoryTests
{
    private async Task<UserDataRepository> GetRepositoryAsync()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
            .Options;

        var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        return new UserDataRepository(context);
    }

    [Fact]
    public async Task AddUserDataAsync_ShouldAddUser()
    {
        var repo = await GetRepositoryAsync();

        var user = new UserData
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Password = "pass",
            PasswordConfirm = "pass",      
            PhoneNumber = "123-456-7890",  
            IsMentorApplicant = false      
        };

        await repo.AddUserDataAsync(user);

        var retrieved = await repo.GetUserAsync("test@example.com", "pass");

        Assert.NotNull(retrieved);
        Assert.Equal("Test", retrieved.FirstName);
    }
    
    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUser()
    {
        var repo = await GetRepositoryAsync();

        var user = new UserData
        {
            FirstName = "Old",
            LastName = "Name",
            Email = "update@example.com",
            Password = "pass",
            PasswordConfirm = "pass",      
            PhoneNumber = "123-456-7890", 
            IsMentorApplicant = false
        };

        await repo.AddUserDataAsync(user);

        user.FirstName = "New";
        user.Password = "newpass";

        await repo.UpdateUserAsync(user);

        var updated = await repo.GetUserAsync("update@example.com", "newpass");

        Assert.NotNull(updated);
        Assert.Equal("New", updated.FirstName);
    }
    
    [Fact]
    public async Task EmailExistsAsync_ShouldReturnTrueIfExists()
    {
        var repo = await GetRepositoryAsync();

        var user = new UserData
        {
            FirstName = "Test",
            LastName = "User",
            Email = "exists@example.com",
            Password = "pass",
            PasswordConfirm = "pass",
            PhoneNumber = "123-456-7890",
            IsMentorApplicant = false
        };
        await repo.AddUserDataAsync(user);

        var exists = await repo.EmailExistsAsync("exists@example.com");
        var notExists = await repo.EmailExistsAsync("nope@example.com");

        Assert.True(exists);
        Assert.False(notExists);
    }
    
    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        var repo = await GetRepositoryAsync();

        await repo.AddUserDataAsync(new UserData
        {
            FirstName = "A",
            LastName = "User",
            Email = "a@a.com",
            Password = "pass",
            PasswordConfirm = "pass",
            PhoneNumber = "123-456-7890",
            IsMentorApplicant = false
        });

        await repo.AddUserDataAsync(new UserData
        {
            FirstName = "B",
            LastName = "User",
            Email = "b@b.com",
            Password = "pass",
            PasswordConfirm = "pass",
            PhoneNumber = "123-456-7890",
            IsMentorApplicant = false
        });
        var users = await repo.GetAllUsersAsync();

        Assert.Equal(2, users.Count);
    }

    [Fact]
    public async Task GetUserAsync_ShouldReturnNullIfNotFound()
    {
        var repo = await GetRepositoryAsync();

        var user = await repo.GetUserAsync("missing@example.com", "pass");

        Assert.Null(user);
    }
}