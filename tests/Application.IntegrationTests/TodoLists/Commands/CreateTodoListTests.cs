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
        using var scope = Testing.ScopeFactory.CreateScope();
        var validator = new CreateTodoListCommandValidator(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());

        var command = new CreateTodoListCommand();

        var result = await validator.ValidateAsync(command);
        Assert.IsFalse(result.IsValid);

    }

    [Test]
    public async Task ShouldRequireUniqueTitle()
    {
        //Setup
        await SendAsync(new CreateTodoListCommand
        {
            Title = "Shopping"
        });


        //Test
        using var scope = Testing.ScopeFactory.CreateScope();
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
        var userId = await RunAsDefaultUserAsync();

        var command = new CreateTodoListCommand
        {
            Title = "Tasks"
        };

        var id = await SendAsync(command);

        var list = await FindAsync<TodoList>(id);

        list.Should().NotBeNull();
        list!.Title.Should().Be(command.Title);
        list.CreatedBy.Should().Be(userId);
        list.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
