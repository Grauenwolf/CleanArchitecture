using MediatR;

namespace CleanArchitecture.Application.WeatherForecasts.Queries.GetWeatherForecasts;

public class GetWeatherForecastsQuery : IRequest<IEnumerable<WeatherForecast>>
{
}
