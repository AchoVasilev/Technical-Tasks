namespace Seeding;

using Data.Json;
using Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using XmlDbContext;

public static class ApplicationBuilderExtensions
{
    public static async Task<IApplicationBuilder> PrepareJsonDatabase(this IApplicationBuilder builder)
    {
        using var scopedServices = builder.ApplicationServices.CreateScope();
        var serviceProvider = scopedServices.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<JsonDbContext>();

        await SeedWeatherToJsonDatabase(dbContext);
        return builder;
    }
    
    public static async Task<IApplicationBuilder> PrepareXmlDatabase(this IApplicationBuilder builder)
    {
        using var scopedServices = builder.ApplicationServices.CreateScope();
        var serviceProvider = scopedServices.ServiceProvider;
        var dbContext = serviceProvider.GetRequiredService<XmlDbContext>();

        await SeedWeatherToXmlDatabase(dbContext);
        return builder;
    }

    private static async Task SeedWeatherToJsonDatabase(JsonDbContext dbContext)
    {
        if (dbContext.Weathers.Any())
        {
            return;
        }
        
        var data = new List<Weather>()
        {
            new Weather() { Pressure = 15, Temperature = 16},
            new Weather() { Pressure = 10, Temperature = -5},
            new Weather() { Pressure = 12, Temperature = 11},
            new Weather() { Pressure = 11, Temperature = 15},
            new Weather() { Pressure = 14, Temperature = 25},
            new Weather() { Pressure = 18, Temperature = 24},
            new Weather() { Pressure = 13, Temperature = 19},
            new Weather() { Pressure = 9, Temperature = 21},
            new Weather() { Pressure = 8, Temperature = 22},
            new Weather() { Pressure = 4, Temperature = 20},
        };

        await dbContext.AddRangeAsync(data);
        await dbContext.SaveChangesAsync();
    }
    
    private static async Task SeedWeatherToXmlDatabase(XmlDbContext dbContext)
    {
        if (dbContext.Weathers.Any())
        {
            return;
        }
        
        var data = new List<Weather>()
        {
            new Weather() { Pressure = 1, Temperature = 13},
            new Weather() { Pressure = 2, Temperature = 14},
            new Weather() { Pressure = 5, Temperature = -20},
            new Weather() { Pressure = 8, Temperature = 31},
            new Weather() { Pressure = 10, Temperature = 34},
            new Weather() { Pressure = 12, Temperature = -15},
            new Weather() { Pressure = 14, Temperature = 22},
            new Weather() { Pressure = 16, Temperature = 36},
            new Weather() { Pressure = 3, Temperature = -3},
            new Weather() { Pressure = 4, Temperature = 20},
        };

        await dbContext.AddRangeAsync(data);
        await dbContext.SaveChangesAsync();
    }
}