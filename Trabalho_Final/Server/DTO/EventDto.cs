namespace Server.DTO;

public class EventDto
{
    public int Id { get; set; }
    
    public int OrganizerId { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime EventDate { get; set; }
    
    public string Location { get; set; }
    
    public int Capacity { get; set; }
    
    public decimal Price { get; set; }
    
    public string? Category { get; set; }
}