using Data.Json;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Models;
using Seeding;
using Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<JsonDbContext>(options =>
    options.UseSqlServer(connectionString));
// Add services to the container.
builder.Services.AddTransient<IJsonDbService, JsonDbService>();
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

app.PrepareJsonDatabase()
    .GetAwaiter()
    .GetResult();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
