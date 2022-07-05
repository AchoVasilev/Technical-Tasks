namespace XmlDbContext;

using Data.Models;
using Microsoft.EntityFrameworkCore;

public class XmlDbContext : DbContext
{
    public XmlDbContext(DbContextOptions<XmlDbContext> options)
        : base(options)
    { }
    
    public DbSet<Weather> Weathers { get; set; }
}