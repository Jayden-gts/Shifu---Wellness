using Microsoft.EntityFrameworkCore;

namespace Shifu.Models;
// Created by Jayden Seto - 991746683
// This is a Model representing the EFC database context. Managing entity sets.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserData> Users { get; set; }
    
    // check this later 
    public DbSet<Message> Messages { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Resource> Resources { get; set; }
}