using AutoMapper;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.TodoLists;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Commands;

using static Testing;

public class CreateTodoListTests : TestBase
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        using var scope = ScopeFactory.CreateScope();
        var validator = new CreateTodoListCommandValidator(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());

        var command = new CreateTodoListCommand();

        var result = await validator.ValidateAsync(command);
        Assert.IsFalse(result.IsValid);

    }

    [Test]
    public async Task ShouldRequireUniqueTitle()
    {
        //Setup
        var userId = await RunAsDefaultUserAsync();
        using var scope = ScopeFactory.CreateScope();
        var service = new TodoListService(
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
            scope.ServiceProvider.GetRequiredService<IMapper>(),
            scope.ServiceProvider.GetRequiredService<ICsvFileBuilder>()
            );


        await service.Create(new CreateTodoListCommand
        {
            Title = "Shopping"
        }, CancellationToken.None);


        //Test
        var validator = new CreateTodoListCommandValidator(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());

        var command = new CreateTodoListCommand
        {
            Title = "Shopping"
        };

        var result = await validator.ValidateAsync(command);
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public async Task ShouldCreateTodoList()
    {
        //Setup
        var userId = await RunAsDefaultUserAsync();
        using var scope = ScopeFactory.CreateScope();
        var service = new TodoListService(
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
            scope.ServiceProvider.GetRequiredService<IMapper>(),
            scope.ServiceProvider.GetRequiredService<ICsvFileBuilder>()
            );

        //Test

        var command = new CreateTodoListCommand
        {
            Title = "Tasks"
        };

        var id = await service.Create(command, CancellationToken.None);

        var list = await FindAsync<TodoList>(id);

        list.Should().NotBeNull();
        list!.Title.Should().Be(command.Title);
        list.CreatedBy.Should().Be(userId);
        list.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
