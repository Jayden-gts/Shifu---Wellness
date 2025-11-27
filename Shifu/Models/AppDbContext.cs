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
    public DbSet<UserMentorAssignment> UserMentorAssignments { get; set; }
    
    
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Resource> Resources { get; set; }
    
    }

    
