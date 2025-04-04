using Server.Data;
using Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTO;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventEntity = await _context.Events
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

            if (eventEntity == null)
            {
                return NotFound(new { message = "Event not found" });
            }

            var eventDto = new EventDto
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

            return Ok(eventDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
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

            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
        {
            var eventEntity = new Event
            {
                OrganizerId = eventDto.OrganizerId,
                Name = eventDto.Name,
                Description = eventDto.Description,
                EventStartDate = eventDto.EventStartDate,
                EventEndDate = eventDto.EventEndDate,
                Location = eventDto.Location,
                Capacity = eventDto.Capacity,
                Category = eventDto.Category,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Events.Add(eventEntity);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Evento criado com sucesso!",
                eventId = eventEntity.Id
            });
        }
    }

}