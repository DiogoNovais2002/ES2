namespace Server.DTO;

public class EventDto
{
    public int id { get; set; }
    
    public int organizerid { get; set; }
    
    public string name  { get; set; }
    
    public string description { get; set; }
    
    public DateTime eventdate  { get; set; }
    
    public string locacion   { get; set; }
    
    public int capacity   { get; set; }
    
    public decimal price    { get; set; }
    
    public string category   { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}