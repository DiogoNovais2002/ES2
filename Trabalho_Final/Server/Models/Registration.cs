namespace Server.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public int TicketId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; }
        public Event? Event { get; set; }
        public User? User { get; set; }
        public EventTicket? Ticket { get; set; }
    }
}