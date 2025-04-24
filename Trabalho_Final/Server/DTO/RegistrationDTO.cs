namespace Server.DTO;

public class RegistrationDto
{
    public int UserId { get; set; }
    public int EventId { get; set; }
    public int TicketId { get; set; } // Pode ser opcional, mas mantemos aqui
}

