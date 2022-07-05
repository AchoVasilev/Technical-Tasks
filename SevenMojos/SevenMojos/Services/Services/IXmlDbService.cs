namespace Services;

using Models;

public interface IXmlDbService
{
    Task<XmlWeatherViewModel> GetWeatherDataAsync();
}