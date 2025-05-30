namespace Server.Models;

public class OrganizerAnnouncement
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public Event Event { get; set; }
}