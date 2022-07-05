namespace Data.Json;

using Microsoft.EntityFrameworkCore;
using Models;

public class JsonDbContext : DbContext
{
    public JsonDbContext(DbContextOptions<JsonDbContext> options)
        : base(options)
    {}
    
    public DbSet<Weather> Weathers { get; set; }
}