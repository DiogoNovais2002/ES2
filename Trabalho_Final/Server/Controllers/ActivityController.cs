using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2.Data;
using Server.DTO;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Activity
        [HttpGet]
        public async Task<IActionResult> GetActivities()
        {
            var activities = await _context.Activities
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

            return Ok(activities);
        }

        // GET: api/Activity/event/5
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetActivitiesByEventId(int eventId)
        {
            var activities = await _context.Activities
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

            return Ok(activities);
        }

        // POST: api/Activity
        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] ActivityDto dto)
        {
            var activity = new Activity
            {
                EventId = dto.EventId,
                Name = dto.Name,
                Description = dto.Description,
                ActivityStartDate = dto.ActivityStartDate,
                ActivityEndDate = dto.ActivityEndDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Atividade criada com sucesso!",
                activityId = activity.Id
            });
        }

        // PUT: api/Activity/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] ActivityDto dto)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
                return NotFound(new { message = "Atividade não encontrada." });

            activity.Name = dto.Name;
            activity.Description = dto.Description;
            activity.ActivityStartDate = dto.ActivityStartDate;
            activity.ActivityEndDate = dto.ActivityEndDate;
            activity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Atividade atualizada com sucesso." });
        }

        // DELETE: api/Activity/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
                return NotFound(new { message = "Atividade não encontrada." });

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Atividade eliminada com sucesso." });
        }
    }
}
