using DataSource.Xml;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Seeding;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<XmlDbContext.XmlDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddTransient<IXmlDbService, XmlDbService>();
builder.Services.AddControllers(options =>
    {
        options.Filters.Add(new ProducesAttribute("text/xml"));
        options.OutputFormatters.Add(new XmlSerializerOutputFormatterNamespace());
    })
    .AddXmlSerializerFormatters();

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.Configure<ApplicationSettingsModel>(builder.Configuration.GetSection("Application"));
builder.Services.AddMemoryCache();
builder.Services.AddAuthentication(o => 
    o.AddScheme("api", a => a.HandlerType = typeof(HmacAuthenticationHandler)));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.PrepareXmlDatabase()
    .GetAwaiter()
    .GetResult();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
