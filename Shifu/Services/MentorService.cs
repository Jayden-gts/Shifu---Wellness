namespace Shifu.Services;

using Shifu.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MentorService
{
    
    // the mentor services enable 
    private readonly AppDbContext _db;
    public MentorService(AppDbContext  db)
    {
        _db = db;
    }

    public async Task<List<Message>> GetMessagesForUser(int userId)
    {
        return await _db.Messages
            .Where(m => m.UserId == userId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
    }

    public async Task SaveMessage(int userId, string content)
    {
        var message = new Message
        {
            UserId = userId,
            SentByMentor = true,
            Content = content,
            Timestamp = DateTime.Now
        };
        
        _db.Messages.Add(message);
        await _db.SaveChangesAsync();
    }

}