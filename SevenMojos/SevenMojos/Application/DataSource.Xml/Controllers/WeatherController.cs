using Microsoft.AspNetCore.Mvc;

namespace DataSource.Xml.Controllers;

using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Models;
using Services;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = "api")]
public class WeatherController : ControllerBase
{
    private readonly IXmlDbService dbService;

    public WeatherController(IXmlDbService dbService)
    {
        this.dbService = dbService;
    }

    [HttpGet]
    [Route("today")]
    public async Task<ActionResult> Get()
    {
        var result = await this.dbService.GetWeatherDataAsync();
        var response = new XmlResponseModel();

        if (result is null)
        {
            response.Error = 2;
            response.Success = false;
        }
        else if (DateTime.UtcNow.Hour > Constants.XmlEndHour && Constants.XmlStartHour < DateTime.UtcNow.Hour)
        {
            response.Error = 3;
            response.Success = false;
        }
        else
        {
            response.Data = result;
            response.Error = 0;
        }

        return this.Ok(response);
    }
}