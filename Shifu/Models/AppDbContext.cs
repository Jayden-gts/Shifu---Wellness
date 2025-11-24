using Microsoft.EntityFrameworkCore;

namespace Shifu.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserData> Users { get; set; }
    
    // check this later 
    public DbSet<Message> Messages { get; set; }
}