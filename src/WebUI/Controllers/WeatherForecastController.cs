using CleanArchitecture.Application.WeatherForecasts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebUI.Controllers;

public class WeatherForecastController : ApiControllerBase
{
    readonly WeatherForecastService _weatherForecastService;
    public WeatherForecastController(WeatherForecastService weatherForecastService)
    {
        _weatherForecastService = weatherForecastService;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get(CancellationToken cancellationToken)
    {
        return await _weatherForecastService.GetForecasts(new GetWeatherForecastsQuery(), cancellationToken);
    }
}
