using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTO;
using Server.Models;

namespace Server.Services
{
    public class EventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EventDto?> GetByIdAsync(int id)
        {
            var eventEntity = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (eventEntity == null) return null;

            return new EventDto
            {
                Id = eventEntity.Id,
                OrganizerId = eventEntity.OrganizerId,
                Name = eventEntity.Name,
                Description = eventEntity.Description,
                EventStartDate = eventEntity.EventStartDate,
                EventEndDate = eventEntity.EventEndDate,
                Location = eventEntity.Location,
                Capacity = eventEntity.Capacity,
                Category = eventEntity.Category
            };
        }

        public async Task<List<EventDto>> GetAllAsync()
        {
            return await _context.Events
                .Select(e => new EventDto
                {
                    Id = e.Id,
                    OrganizerId = e.OrganizerId,
                    Name = e.Name,
                    Description = e.Description,
                    EventStartDate = e.EventStartDate,
                    EventEndDate = e.EventEndDate,
                    Location = e.Location,
                    Capacity = e.Capacity,
                    Category = e.Category
                })
                .ToListAsync();
        }

        public async Task<int> CreateAsync(EventDto dto)
        {
            var eventEntity = new Event
            {
                OrganizerId = dto.OrganizerId,
                Name = dto.Name,
                Description = dto.Description,
                EventStartDate = dto.EventStartDate,
                EventEndDate = dto.EventEndDate,
                Location = dto.Location,
                Capacity = dto.Capacity,
                Category = dto.Category,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity.Id;
        }
        
        public async Task<bool> UpdateAsync(int id, EventDto dto)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity == null)
                return false;

            eventEntity.Name = dto.Name;
            eventEntity.Description = dto.Description;
            eventEntity.EventStartDate = dto.EventStartDate;
            eventEntity.EventEndDate = dto.EventEndDate;
            eventEntity.Location = dto.Location;
            eventEntity.Capacity = dto.Capacity;
            eventEntity.Category = dto.Category;
            eventEntity.UpdatedAt = DateTime.UtcNow;

            _context.Events.Update(eventEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        
        public async Task<List<string?>> GetCategoriesAsync()
        {
            return await _context.Events
                .Select(e => e.Category)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetLocalidadesAsync()
        {
            return await _context.Events
                .Select(e => e.Location)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<string>> GetDatasAsync()
        {
            var datas = await _context.Events
                .Select(e => e.EventStartDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            return datas.Select(d => d.ToString("yyyy-MM-dd")).ToList();
        }

    }
}
