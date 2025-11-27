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

    public async Task SaveMessage(int userId, string content, bool sentByMentor, int? mentorId = null)
    {
        var message = new Message
        {
            UserId = userId,
            MentorId = mentorId,
            SentByMentor = true,
            Content = content,
            Timestamp = DateTime.UtcNow
        };
        
        _db.Messages.Add(message);
        await _db.SaveChangesAsync();
    }
    
    public async Task AssignMentor(int userId, int mentorId)
    {
        var existing = await _db.UserMentorAssignments.FirstOrDefaultAsync(a => a.UserId == userId);
        if (existing == null)
        {
            _db.UserMentorAssignments.Add(new UserMentorAssignment { UserId = userId, MentorId = mentorId });
        }
        else
        {
            existing.MentorId = mentorId;
            existing.AssignedAt = System.DateTime.UtcNow;
        }
        await _db.SaveChangesAsync();
    }
    
    public Task<UserMentorAssignment?> GetAssignmentForUser(int userId) => 
        _db.UserMentorAssignments.FirstOrDefaultAsync(a => a.UserId == userId);
    
    public Task<List<UserData>> GetMentorsAvailable() =>
        _db.Users.Where(u => u.IsMentor && u.IsAvailable).ToListAsync();
    
    public Task<List<UserData>> GetMentorsAllApproved() =>
        _db.Users.Where(u => u.IsMentor).ToListAsync();
    
    
    public Task<List<UserData>> GetUsersAssignedToMentor(int mentorId) =>
        _db.UserMentorAssignments
            .Where(a => a.MentorId == mentorId)
            .Join(_db.Users, a => a.UserId, u => u.Id, (a,u) => u)
            .ToListAsync();

}