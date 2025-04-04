namespace ES2.Data
{
    public class EventTicket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string TicketType { get; set; }
        public decimal Price { get; set; }
        public int QuantityAvailable { get; set; }
        
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Event? Event { get; set; }
    }
}