namespace Services;

using Data.Json;
using Microsoft.EntityFrameworkCore;
using Models;

public class JsonDbService : IJsonDbService
{
    private readonly JsonDbContext dbcontext;

    public JsonDbService(JsonDbContext dbContext)
    {
        this.dbcontext = dbContext;
    }
    
    public async Task<JsonWeatherViewModel> GetWeatherDataAsync()
    {
        var random = new Random();
        var number = random.Next(1, 10);
        return await this.dbcontext.Weathers
            .Select(x => new JsonWeatherViewModel()
            {
                Temperature = x.Temperature,
                Pressure = x.Pressure
            })
            .Skip(number)
            .Take(1)
            .SingleAsync();
    }
}