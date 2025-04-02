namespace Server.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public Event? Event { get; set; }
        public User? User { get; set; }
    }
}