using System.Reflection;
using CleanArchitecture.Application.TodoItems;
using CleanArchitecture.Application.TodoLists;
using CleanArchitecture.Application.WeatherForecasts;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddSingleton<GetWeatherForecastsQueryHandler>();

        services.AddTransient<GetTodoItemsWithPaginationQueryHandler>();
        services.AddTransient<CreateTodoItemCommandHandler>();
        services.AddTransient<UpdateTodoItemCommandHandler>();
        services.AddTransient<UpdateTodoItemDetailCommandHandler>();
        services.AddTransient<DeleteTodoItemCommandHandler>();
        services.AddTransient<GetTodosQueryHandler>();
        services.AddTransient<ExportTodosQueryHandler>();
        services.AddTransient<CreateTodoListCommandHandler>();
        services.AddTransient<UpdateTodoListCommandHandler>();
        services.AddTransient<DeleteTodoListCommandHandler>();


        return services;
    }
}
