using Microsoft.EntityFrameworkCore;

namespace Shifu.Models;
// Created by Jayden Seto - 991746683
// This Model is a user repository to help structure the database operations for a user
public class UserDataRepository
{
    private readonly AppDbContext _context;

    public UserDataRepository(AppDbContext context)
    {
        _context = context;
    }

    // Adds a new user to the database
    public async Task AddUserDataAsync(UserData user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    //Updates existing user, used for editing user profile
    public async Task UpdateUserAsync(UserData userData)
    {
        var existing = await _context.Users.FindAsync(userData.Id);
        if (existing == null)
        {
            throw new Exception("No user found");
        }
        
        existing.FirstName = userData.FirstName;
        existing.LastName = userData.LastName;
        existing.Email = userData.Email;
        existing.PhoneNumber = userData.PhoneNumber;

        if (!string.IsNullOrEmpty(userData.Password))
        {
            existing.Password = userData.Password;
        }

        await _context.SaveChangesAsync();

    }

    // Ensures unique email per account, prompts log in if email exists
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    //Retrieve user by email and password
    public async Task<UserData?> GetUserAsync(string email, string password)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
    }

    //Retrieve all users
    public async Task<List<UserData>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    } 

}