using Microsoft.EntityFrameworkCore;

namespace Shifu.Models;

public class UserDataRepository
{
    private readonly AppDbContext _context;

    public UserDataRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddUserDataAsync(UserData user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<UserData?> GetUserAsync(string email, string password)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
    }

    public async Task<List<UserData>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    } 

}