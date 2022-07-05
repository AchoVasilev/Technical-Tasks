namespace DataSource.Json.Controllers;

using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "api")]
[DateFilter(Constants.JsonStartHour, Constants.JsonEndHour)]
public class WeatherController : ControllerBase
{
    private readonly IJsonDbService service;

    public WeatherController(IJsonDbService service)
    {
        this.service = service;
    }

    [HttpGet]
    [Route("today")]
    public async Task<ActionResult> Get()
    {
        var data = await this.service.GetWeatherDataAsync();

        if (data is null)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError);
        }

        return this.Ok(data);
    }
}