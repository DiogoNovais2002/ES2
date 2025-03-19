namespace ES2.Data
{
    public class Report
    {
        public int Id { get; set; }
        public int? EventId { get; set; }
        public int? TotalParticipants { get; set; }
        public decimal? Revenue { get; set; }
        public string? Feedback { get; set; }
        public DateTime GeneratedAt { get; set; }
        public Event? Event { get; set; }
    }
}