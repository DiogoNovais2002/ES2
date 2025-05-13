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
            var e = await _context.Events.FindAsync(id);
            return e == null ? null : MapToDto(e);
        }

        public async Task<List<EventDto>> GetAllAsync()
        {
            return await _context.Events
                .Select(e => MapToDto(e))
                .ToListAsync();
        }

        public async Task<int> CreateAsync(EventDto dto)
        {
            var entity = new Event
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

            _context.Events.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(int id, EventDto dto)
        {
            var e = await _context.Events.FindAsync(id);
            if (e == null) return false;

            e.Name = dto.Name;
            e.Description = dto.Description;
            e.EventStartDate = dto.EventStartDate;
            e.EventEndDate = dto.EventEndDate;
            e.Location = dto.Location;
            e.Capacity = dto.Capacity;
            e.Category = dto.Category;
            e.UpdatedAt = DateTime.UtcNow;

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

        public async Task<(bool Success, string Message)> ParticipateAsync(RegistrationDto dto)
        {
            // Verifica se o evento existe
            var evento = await _context.Events.FindAsync(dto.EventId);
            if (evento == null)
                return (false, "Evento não encontrado.");

            // Verifica se o bilhete existe e pertence ao evento
            var ticket = await _context.EventTickets
                .FirstOrDefaultAsync(t => t.Id == dto.TicketId && t.EventId == dto.EventId);

            if (ticket == null)
                return (false, "Bilhete inválido para este evento.");

            // Verifica se o utilizador já está inscrito
            var jaInscrito = await _context.Registrations
                .AnyAsync(r => r.UserId == dto.UserId && r.EventId == dto.EventId);

            if (jaInscrito)
                return (false, "Já estás inscrito neste evento.");

            // Verifica se há bilhetes disponíveis
            if (ticket.QuantityAvailable <= 0)
                return (false, "Bilhete esgotado.");

            // Cria a inscrição
            var registration = new Registration
            {
                UserId = dto.UserId,
                EventId = dto.EventId,
                TicketId = dto.TicketId,
                RegistrationDate = DateTime.UtcNow,
                Status = "Ativa"
            };

            // Reduz a quantidade disponível
            ticket.QuantityAvailable--;

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();

            return (true, "Inscrição realizada com sucesso.");
        }


        public async Task<List<EventDto>> GetEventsByParticipantAsync(int userId)
        {
            return await _context.Registrations
                .Where(r => r.UserId == userId)
                .Include(r => r.Event)
                .Select(r => MapToDto(r.Event))
                .ToListAsync();
        }

        public async Task<bool> CancelParticipationAsync(int eventId, int userId)
        {
            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

            if (registration == null)
                return false;

            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();
            return true;
        }

        private static EventDto MapToDto(Event e)
        {
            return new EventDto
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
            };
        }
        public async Task<EventReportDto> GetReportAsync()
        {
            var eventos = await _context.Events.ToListAsync();
            var registros = await _context.Registrations.ToListAsync();
            var atividades = await _context.Activities.ToListAsync(); 

            var report = new EventReportDto
            {
                TotalEventos = eventos.Count,
                EventoMaisCaro = eventos.OrderByDescending(e => e.Capacity).FirstOrDefault()?.Name,
                EventoComMaisParticipantes = registros
                    .GroupBy(r => r.EventId)
                    .OrderByDescending(g => g.Count())
                    .Join(eventos, g => g.Key, e => e.Id, (g, e) => e.Name)
                    .FirstOrDefault(),

                EventoComMaisAtividades = atividades
                    .GroupBy(a => a.EventId)
                    .OrderByDescending(g => g.Count())
                    .Join(eventos, g => g.Key, e => e.Id, (g, e) => e.Name)
                    .FirstOrDefault(),

                EventoMaisLongo = eventos
                    .OrderByDescending(e => (e.EventEndDate - e.EventStartDate).TotalHours)
                    .FirstOrDefault()?.Name,

                MediaParticipantes = eventos.Count > 0
                    ? registros.Count / (double)eventos.Count
                    : 0,

                Categorias = eventos
                    .GroupBy(e => e.Category)
                    .ToDictionary(g => g.Key ?? "Sem categoria", g => g.Count()),

                Localidades = eventos
                    .GroupBy(e => e.Location)
                    .ToDictionary(g => g.Key ?? "Sem localidade", g => g.Count()),

            };

            return report;
        }
        
        public async Task<EventDetailReportDto?> GetReportByEventAsync(int eventId)
        {
            var ev = await _context.Events
                .Include(e => e.Registrations)
                .Include(e => e.Activities)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (ev == null) 
                return null;

            return new EventDetailReportDto
            {
                // básicos
                Id               = ev.Id,
                OrganizerId      = ev.OrganizerId,
                Name             = ev.Name,
                Description      = ev.Description,
                EventStartDate   = ev.EventStartDate,
                EventEndDate     = ev.EventEndDate,
                Location         = ev.Location,
                Capacity         = ev.Capacity,
                Category         = ev.Category,

                // métricas
                TotalParticipants = ev.Registrations.Count,
                TotalActivities   = ev.Activities.Count,
                DurationHours = ev.Activities.Count > 0
                    ? ev.Activities.Sum(a => (a.ActivityEndDate - a.ActivityStartDate).TotalHours)
                    : (ev.EventEndDate - ev.EventStartDate).TotalHours
            };
        }




    }
}
