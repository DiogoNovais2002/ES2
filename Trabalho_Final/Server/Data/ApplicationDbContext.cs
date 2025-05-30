// ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Models;
namespace Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventTicket> EventTickets { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<ActivityRegistration> ActivityRegistrations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<OrganizerAnnouncement> OrganizerAnnouncements { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("public");

            builder.Entity<User>().ToTable("users");
            builder.Entity<User>().Property(u => u.Id).HasColumnName("id");
            builder.Entity<User>().Property(u => u.Name).HasColumnName("name");
            builder.Entity<User>().Property(u => u.PhoneNumber).HasColumnName("phonenumber");
            builder.Entity<User>().Property(u => u.UserType).HasColumnName("usertype");
            builder.Entity<User>().Property(u => u.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("NOW()");
            builder.Entity<User>().Property(u => u.UpdatedAt).HasColumnName("updatedat").HasDefaultValueSql("NOW()");

            builder.Entity<Event>().ToTable("events");
            builder.Entity<Event>().Property(e => e.Id).HasColumnName("id");
            builder.Entity<Event>().Property(e => e.OrganizerId).HasColumnName("organizerid");
            builder.Entity<Event>().Property(e => e.Name).HasColumnName("name");
            builder.Entity<Event>().Property(e => e.Description).HasColumnName("description");
            builder.Entity<Event>().Property(e => e.EventStartDate).HasColumnName("eventstartdate");
            builder.Entity<Event>().Property(e => e.EventEndDate).HasColumnName("eventenddate");
            builder.Entity<Event>().Property(e => e.Location).HasColumnName("location");
            builder.Entity<Event>().Property(e => e.Capacity).HasColumnName("capacity");
            builder.Entity<Event>().Property(e => e.Category).HasColumnName("category");
            builder.Entity<Event>().Property(e => e.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("NOW()");
            builder.Entity<Event>().Property(e => e.UpdatedAt).HasColumnName("updatedat").HasDefaultValueSql("NOW()");
            builder.Entity<Event>().HasOne(e => e.Organizer).WithMany().HasForeignKey(e => e.OrganizerId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Event>()
                .HasMany(e => e.Activities)
                .WithOne(a => a.Event)
                .HasForeignKey(a => a.EventId);

            builder.Entity<EventTicket>().ToTable("eventtickets");
            builder.Entity<EventTicket>().Property(et => et.Id).HasColumnName("id");
            builder.Entity<EventTicket>().Property(et => et.EventId).HasColumnName("eventid");
            builder.Entity<EventTicket>().Property(et => et.TicketType).HasColumnName("tickettype");
            builder.Entity<EventTicket>().Property(et => et.Price).HasColumnName("price");
            builder.Entity<EventTicket>().Property(et => et.QuantityAvailable).HasColumnName("quantityavailable");
            builder.Entity<EventTicket>().Property(et => et.Description).HasColumnName("description");
            builder.Entity<EventTicket>().Property(et => et.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("NOW()");
            builder.Entity<EventTicket>().Property(et => et.UpdatedAt).HasColumnName("updatedat").HasDefaultValueSql("NOW()");
            builder.Entity<EventTicket>().HasOne(et => et.Event).WithMany().HasForeignKey(et => et.EventId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Activity>().ToTable("activities");
            builder.Entity<Activity>().Property(a => a.Id).HasColumnName("id");
            builder.Entity<Activity>().Property(a => a.EventId).HasColumnName("eventid");
            builder.Entity<Activity>().Property(a => a.Name).HasColumnName("name");
            builder.Entity<Activity>().Property(a => a.Description).HasColumnName("description");
            builder.Entity<Activity>().Property(a => a.ActivityStartDate).HasColumnName("activitystartdate");
            builder.Entity<Activity>().Property(a => a.ActivityEndDate).HasColumnName("activityenddate");
            builder.Entity<Activity>().Property(a => a.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("NOW()");
            builder.Entity<Activity>().Property(a => a.UpdatedAt).HasColumnName("updatedat").HasDefaultValueSql("NOW()");
            builder.Entity<Activity>().HasOne(a => a.Event).WithMany(e => e.Activities).HasForeignKey(a => a.EventId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Registration>().ToTable("registrations");
            builder.Entity<Registration>().Property(r => r.Id).HasColumnName("id");
            builder.Entity<Registration>().Property(r => r.EventId).HasColumnName("eventid");
            builder.Entity<Registration>().Property(r => r.UserId).HasColumnName("userid");
            builder.Entity<Registration>().Property(r => r.TicketId).HasColumnName("ticketid");
            builder.Entity<Registration>().Property(r => r.RegistrationDate).HasColumnName("registrationdate").HasDefaultValueSql("NOW()");
            builder.Entity<Registration>().Property(r => r.Status).HasColumnName("status").HasDefaultValue("Active");
            builder.Entity<Registration>().HasOne(r => r.Event).WithMany(e => e.Registrations).HasForeignKey(r => r.EventId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Registration>().HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Registration>().HasOne(r => r.Ticket).WithMany().HasForeignKey(r => r.TicketId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ActivityRegistration>().ToTable("activityregistrations");
            builder.Entity<ActivityRegistration>().Property(ar => ar.Id).HasColumnName("id");
            builder.Entity<ActivityRegistration>().Property(ar => ar.RegistrationId).HasColumnName("registrationid");
            builder.Entity<ActivityRegistration>().Property(ar => ar.ActivityId).HasColumnName("activityid");
            builder.Entity<ActivityRegistration>().Property(ar => ar.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("NOW()");
            builder.Entity<ActivityRegistration>().HasOne(ar => ar.Registration).WithMany().HasForeignKey(ar => ar.RegistrationId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ActivityRegistration>().HasOne(ar => ar.Activity).WithMany().HasForeignKey(ar => ar.ActivityId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>().ToTable("messages");
            builder.Entity<Message>().Property(m => m.Id).HasColumnName("id");
            builder.Entity<Message>().Property(m => m.EventId).HasColumnName("eventid");
            builder.Entity<Message>().Property(m => m.SenderId).HasColumnName("senderid");
            builder.Entity<Message>().Property(m => m.Content).HasColumnName("content");
            builder.Entity<Message>().Property(m => m.SentAt).HasColumnName("sentat").HasDefaultValueSql("NOW()");
            builder.Entity<Message>().HasOne(m => m.Event).WithMany().HasForeignKey(m => m.EventId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Message>().HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Report>().ToTable("reports");
            builder.Entity<Report>().Property(r => r.Id).HasColumnName("id");
            builder.Entity<Report>().Property(r => r.EventId).HasColumnName("eventid");
            builder.Entity<Report>().Property(r => r.TotalParticipants).HasColumnName("totalparticipants");
            builder.Entity<Report>().Property(r => r.Revenue).HasColumnName("revenue");
            builder.Entity<Report>().Property(r => r.Feedback).HasColumnName("feedback");
            builder.Entity<Report>().Property(r => r.GeneratedAt).HasColumnName("generatedat").HasDefaultValueSql("NOW()");
            builder.Entity<Report>().HasOne(r => r.Event).WithMany().HasForeignKey(r => r.EventId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Feedback>().ToTable("feedback");
            builder.Entity<Feedback>().Property(f => f.Id).HasColumnName("id");
            builder.Entity<Feedback>().Property(f => f.EventId).HasColumnName("eventid");
            builder.Entity<Feedback>().Property(f => f.UserId).HasColumnName("userid");
            builder.Entity<Feedback>().Property(f => f.Rating).HasColumnName("rating");
            builder.Entity<Feedback>().Property(f => f.Comment).HasColumnName("comment");
            builder.Entity<Feedback>().Property(f => f.CreatedAt).HasColumnName("createdat").HasDefaultValueSql("NOW()");
            builder.Entity<Feedback>().HasOne(f => f.Event).WithMany().HasForeignKey(f => f.EventId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Feedback>().HasOne(f => f.User).WithMany().HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Permission>().ToTable("permissions");
            builder.Entity<Permission>().Property(p => p.Id).HasColumnName("id");
            builder.Entity<Permission>().Property(p => p.UserId).HasColumnName("userid");
            builder.Entity<Permission>().Property(p => p.CanCreateEvents).HasColumnName("cancreateevents").HasDefaultValue(false);
            builder.Entity<Permission>().Property(p => p.CanManageUsers).HasColumnName("canmanageusers").HasDefaultValue(false);
            builder.Entity<Permission>().Property(p => p.CanViewReports).HasColumnName("canviewreports").HasDefaultValue(false);
            builder.Entity<Permission>().HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<OrganizerAnnouncement>().ToTable("organizerannouncements");
            builder.Entity<OrganizerAnnouncement>().Property(o => o.Id).HasColumnName("id");
            builder.Entity<OrganizerAnnouncement>().Property(o => o.EventId).HasColumnName("eventid");
            builder.Entity<OrganizerAnnouncement>().Property(o => o.Title).HasColumnName("title");
            builder.Entity<OrganizerAnnouncement>().Property(o => o.Content).HasColumnName("content");
            builder.Entity<OrganizerAnnouncement>().Property(o => o.SentAt).HasColumnName("sentat").HasDefaultValueSql("NOW()");
            builder.Entity<OrganizerAnnouncement>()
                .HasOne(o => o.Event)
                .WithMany()
                .HasForeignKey(o => o.EventId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}