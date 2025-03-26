namespace Server.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool CanCreateEvents { get; set; }
        public bool CanManageUsers { get; set; }
        public bool CanViewReports { get; set; }
        public User? User { get; set; }
    }
}