using System.Reflection;
using CleanArchitecture.Application.TodoItems;
using CleanArchitecture.Application.TodoLists;
using CleanArchitecture.Application.WeatherForecasts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddSingleton<WeatherForecastService>();

        services.AddTransient<TodoItemService>();
        services.AddTransient<TodoListService>();


        return services;
    }
}
