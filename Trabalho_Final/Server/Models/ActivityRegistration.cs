namespace ES2.Data
{
    public class ActivityRegistration
    {
        public int Id { get; set; }
        public int RegistrationId { get; set; }
        public int ActivityId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Registration? Registration { get; set; }
        public Activity? Activity { get; set; }
    }
}