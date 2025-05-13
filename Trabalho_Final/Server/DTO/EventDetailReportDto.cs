namespace Server.DTO
{
    public class EventDetailReportDto
    {
        // dados básicos (copiados manualmente)
        public int Id { get; set; }
        public int OrganizerId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string Location { get; set; } = null!;
        public int Capacity { get; set; }
        public string? Category { get; set; }

        // métricas extras
        public int TotalParticipants { get; set; }
        public int TotalActivities { get; set; }
        public double DurationHours { get; set; }
    }
}