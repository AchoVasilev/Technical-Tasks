namespace Data.Models;

public class Weather
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    public int Temperature { get; set; }
    
    public int Pressure { get; set; }
}