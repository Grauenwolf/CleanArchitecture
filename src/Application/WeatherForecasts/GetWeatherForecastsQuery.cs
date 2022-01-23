using MediatR;

namespace CleanArchitecture.Application.WeatherForecasts;

public class GetWeatherForecastsQuery : IRequest<IEnumerable<WeatherForecast>>
{
}
