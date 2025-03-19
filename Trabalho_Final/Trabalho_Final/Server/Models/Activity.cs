// Models/Activity.cs
namespace ES2.Data
{
    public class Activity
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime ActivityDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Event? Event { get; set; }
    }
}