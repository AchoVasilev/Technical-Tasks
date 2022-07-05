namespace Services;

using Models;

public interface IJsonDbService
{
    Task<JsonWeatherViewModel> GetWeatherDataAsync();
}