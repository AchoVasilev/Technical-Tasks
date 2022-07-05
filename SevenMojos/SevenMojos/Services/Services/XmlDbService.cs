namespace Services;

using Microsoft.EntityFrameworkCore;
using Models;
using XmlDbContext;

public class XmlDbService : IXmlDbService
{
    private readonly XmlDbContext dbContext;

    public XmlDbService(XmlDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<XmlWeatherViewModel> GetWeatherDataAsync()
    {
        var random = new Random();
        var number = random.Next(1, 10);

        return await this.dbContext.Weathers
            .Select(x => new XmlWeatherViewModel()
            {
                Pressure = x.Pressure,
                Temperature = x.Temperature
            }).Skip(number)
            .Take(1)
            .SingleAsync();
    }
}