using Microsoft.AspNetCore.Identity;

namespace Server.Models
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public override string? PhoneNumber { get; set; }
        public string UserType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}