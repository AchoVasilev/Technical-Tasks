namespace Models;

using System.Text.Json.Serialization;

public class JsonWeatherViewModel
{
    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }
    
    [JsonPropertyName("temperature")]
    public int Temperature { get; set; }
}