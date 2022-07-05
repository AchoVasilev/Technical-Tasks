using Microsoft.AspNetCore.Mvc;

namespace WebApi.Public.Controllers;

using System.Text.Json;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Xml.Serialization;
using Models;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{
    private readonly IOptions<ApplicationSettingsModel> appSettings;

    public ApiController(IOptions<ApplicationSettingsModel> appSettings)
    {
        this.appSettings = appSettings;
    }

    [HttpGet]
    [Route("today")]
    public async Task<ActionResult> Get()
    {
        var jsonApiUrl = new Uri("https://localhost:7010/");
        var xmlApiUrl = new Uri("https://localhost:7254/");

        var clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

        var customDelegatingHandler = new HMACDelegatingHandler(this.appSettings);
        customDelegatingHandler.InnerHandler = clientHandler;
        var httpClient = new HttpClient(customDelegatingHandler);
        httpClient.BaseAddress = jsonApiUrl;
        httpClient.DefaultRequestHeaders.Clear();

        var jsonResponseMessage = await httpClient.GetAsync("Weather/today");

        if (jsonResponseMessage.IsSuccessStatusCode)
        {
            var weatherResponse = jsonResponseMessage.Content.ReadAsStringAsync().Result;
            var jsonWeatherInfo = JsonSerializer.Deserialize<JsonWeatherViewModel>(weatherResponse);

            if (jsonWeatherInfo.Pressure == 0 || jsonWeatherInfo.Temperature == 0)
            {
                httpClient.BaseAddress = xmlApiUrl;
                httpClient.DefaultRequestHeaders.Clear();

                var xmlResponseMessage = await httpClient.GetAsync("Weather/today");
                var result = DeserializeXml(xmlResponseMessage);

                if (result is null)
                {
                    return this.BadRequest();
                }

                return this.Ok(result);
            }
            else
            {
                if (jsonWeatherInfo is null)
                {
                    return this.BadRequest();
                }

                return this.Ok(jsonWeatherInfo);
            }
        }
        else
        {
            httpClient.BaseAddress = xmlApiUrl;
            httpClient.DefaultRequestHeaders.Clear();

            var xmlResponseMessage = await httpClient.GetAsync("Weather/today");
            var result = DeserializeXml(xmlResponseMessage);

            if (result is null)
            {
                return this.BadRequest();
            }

            return this.Ok(result);
        }
    }

    private XmlWeatherViewModel DeserializeXml(HttpResponseMessage xmlResponseMessage)
    {
        var xmlWeatherResponse = xmlResponseMessage.Content.ReadAsStringAsync().Result;
        var xmlSerializer = new XmlSerializer(typeof(XmlWeatherViewModel));

        using (var reader = new StringReader(xmlWeatherResponse))
        {
            var xmlWeatherInfo = xmlSerializer.Deserialize(reader) as XmlWeatherViewModel;

            return xmlWeatherInfo;
        }
    }
}