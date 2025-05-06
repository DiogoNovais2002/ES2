namespace Server.DTO;

public class EventReportDto
{
    public int TotalEventos { get; set; }
    public string? EventoMaisCaro { get; set; }
    public string? EventoComMaisParticipantes { get; set; }
    public string? EventoComMaisAtividades { get; set; }
    public string? EventoMaisLongo { get; set; }
    public double MediaParticipantes { get; set; }
    public Dictionary<string, int> Categorias { get; set; } = new();
    public Dictionary<string, int> Localidades { get; set; } = new();
    public string? EventoMaisProximo { get; set; }
}