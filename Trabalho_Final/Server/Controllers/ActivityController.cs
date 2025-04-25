using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Organizador")]   // só Organizadores podem gerir atividades
    public class ActivityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Organizador,Participante")]
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

        [HttpGet("event/{eventId}")]
        [Authorize(Roles = "Organizador,Participante")]
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

        [HttpPost]
        public async Task<IActionResult> CreateActivity([FromBody] ActivityDto dto)
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
            return Ok(new { message = "Atividade criada com sucesso." });
        }

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
