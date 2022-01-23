using CleanArchitecture.Application.WeatherForecasts;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebUI.Controllers;

public class WeatherForecastController : ApiControllerBase
{
    readonly GetWeatherForecastsQueryHandler _getWeatherForecastsQueryHandler;
    public WeatherForecastController(GetWeatherForecastsQueryHandler getWeatherForecastsQueryHandler)
    {
        _getWeatherForecastsQueryHandler = getWeatherForecastsQueryHandler;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        return await _getWeatherForecastsQueryHandler.Handle(new GetWeatherForecastsQuery(), CancellationToken.None);
    }
}
