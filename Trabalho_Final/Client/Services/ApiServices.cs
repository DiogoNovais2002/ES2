
namespace Client.Services;
using System.Net.Http.Json;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Obter uma mensagem de teste
    public async Task<string> GetMessageAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<ApiResponse>("api/Test/hello");
        return response?.Message ?? "Erro ao obter a mensagem";
    }

    // Obter todos os eventos
    public async Task<List<EventDto>> GetEventsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<EventDto>>("api/Event") ?? new List<EventDto>();
    }
    
    public async Task<List<string>> GetEventCategoriesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<string>>("api/Event/categories");
    }
    
    public async Task<List<string>> GetLocalidadesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<string>>("api/Event/localidades");
    }
    
    public async Task<List<string>> GetDataAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<string>>("api/Event/datas");
    }
   
    public async Task<bool> ParticiparEventoAsync(int userId, int eventId)
    {
        var registration = new
        {
            UserId = userId,
            EventId = eventId,
            
        };

        var response = await _httpClient.PostAsJsonAsync("api/event/participate", registration);
        return response.IsSuccessStatusCode;
    }

    
    public async Task<List<EventDto>> GetEventsByParticipantAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"api/Event/participant/{userId}");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<List<EventDto>>();
            return result ?? new List<EventDto>();
        }

        return new List<EventDto>();
    }

    public async Task<bool> CancelarParticipacaoAsync(int userId, int eventId)
    {
        var response = await _httpClient.DeleteAsync($"api/Event/{eventId}/participants/{userId}");

        return response.IsSuccessStatusCode;
    }
    
    // Obter um evento específico por ID
    public async Task<EventDto?> GetEventByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<EventDto>($"api/Event/{id}");
    }

    // Criar um evento
    public async Task<int?> CreateEventAsync(EventDto newEvent)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Event", newEvent);

        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<CreateEventResponse>();
        return result?.eventId;
    }
    
    // Criar os Tickets para o Evento
    public async Task<bool> CreateTicketAsync(EventTicketDto ticket)
    {
        var response = await _httpClient.PostAsJsonAsync("api/EventTicket", ticket);
        return response.IsSuccessStatusCode;
    }
    
    // Criar as Atividades para o Evento
    public async Task<bool> CreateActivityAsync(ActivityDto activity)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Activity", activity);
        return response.IsSuccessStatusCode;
    }


    private class ApiResponse
    {
        public string Message { get; set; }
    }
    // DTO correspondente
    public class EventDto
    {
        public int Id { get; set; }
        public int OrganizerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }

        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string Category { get; set; } = string.Empty;
        public object EventDate { get; set; }
    }


    
    public class EventTicketDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityAvailable { get; set; }
        
        public string Description { get; set; } = string.Empty;
    }
    
    private class CreateEventResponse
    {
        public string message { get; set; }
        public int eventId { get; set; }
    }
    
    public class ActivityDto
    {
        public int EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ActivityStartDate { get; set; }
        public DateTime ActivityEndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    
}