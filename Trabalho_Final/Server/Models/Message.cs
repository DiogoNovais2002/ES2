namespace Server.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public Event? Event { get; set; }
        public User? Sender { get; set; }
    }
}