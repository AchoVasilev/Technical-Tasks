namespace Models;

using System.Xml.Serialization;

[XmlType("")]
public class XmlWeatherViewModel
{
    [XmlElement("pressure")]
    public int Pressure { get; set; }
    
    [XmlElement("temperature")]
    public int Temperature { get; set; }
}