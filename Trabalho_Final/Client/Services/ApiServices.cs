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

    // Obter um evento específico por ID
    public async Task<EventDto?> GetEventByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<EventDto>($"api/Event/{id}");
    }

    // Criar um evento
    public async Task<bool> CreateEventAsync(EventDto newEvent)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Event", newEvent);
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
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}