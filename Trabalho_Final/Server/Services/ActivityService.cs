using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTO;
using Server.Models;

namespace Server.Services
{
    public class ActivityService
    {
        private readonly ApplicationDbContext _context;

        public ActivityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ActivityDto>> GetAllAsync()
        {
            return await _context.Activities
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    EventId = a.EventId,
                    Name = a.Name,
                    Description = a.Description ?? "",
                    ActivityStartDate = a.ActivityStartDate,
                    ActivityEndDate = a.ActivityEndDate,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<List<ActivityDto>> GetByEventIdAsync(int eventId)
        {
            return await _context.Activities
                .Where(a => a.EventId == eventId)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    EventId = a.EventId,
                    Name = a.Name,
                    Description = a.Description ?? "",
                    ActivityStartDate = a.ActivityStartDate,
                    ActivityEndDate = a.ActivityEndDate,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task CreateAsync(ActivityDto dto)
        {
            var activity = new Activity
            {
                EventId = dto.EventId,
                Name = dto.Name,
                Description = dto.Description,
                ActivityStartDate = DateTime.SpecifyKind(dto.ActivityStartDate, DateTimeKind.Utc),
                ActivityEndDate = DateTime.SpecifyKind(dto.ActivityEndDate, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, ActivityDto dto)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null) return false;

            activity.Name = dto.Name;
            activity.Description = dto.Description;
            activity.ActivityStartDate = dto.ActivityStartDate;
            activity.ActivityEndDate = dto.ActivityEndDate;
            activity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null) return false;

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
