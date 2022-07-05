namespace Models;

using System.Xml.Serialization;

[XmlType("response")]
public class XmlResponseModel
{
    [XmlElement("success")]
    public bool Success { get; set; } = true;

    [XmlElement("error")]
    public int Error { get; set; } = 0;
    
    [XmlElement("data")]
    public XmlWeatherViewModel? Data { get; set; }
}