// Server/Models/Event.cs
using System;
using System.Collections.Generic;

namespace Server.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int OrganizerId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string Location { get; set; } = null!;
        public int Capacity { get; set; }
        public string? Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public User? Organizer { get; set; }

        // ← Adicionei estas duas propriedades de navegação:
        public virtual ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public virtual ICollection<Activity>    Activities    { get; set; } = new List<Activity>();
    }
}