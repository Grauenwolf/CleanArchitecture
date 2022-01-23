using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.TodoLists;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Commands;

using static Testing;

public class UpdateTodoListTests : TestBase
{
    [Test]
    public async Task ShouldRequireValidTodoListId()
    {
        var command = new UpdateTodoListCommand { Id = 99, Title = "New Title" };
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldRequireUniqueTitle()
    {

        //Setup
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        await SendAsync(new CreateTodoListCommand
        {
            Title = "Other List"
        });


        //Test
        using var scope = Testing.ScopeFactory.CreateScope();
        var validator = new UpdateTodoListCommandValidator(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());

        var command = new UpdateTodoListCommand
        {
            Id = listId,
            Title = "Other List"
        };

        var result = await validator.ValidateAsync(command);
        Assert.IsFalse(result.IsValid);

        Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Title" && e.ErrorMessage == "The specified title already exists."));
    }

    [Test]
    public async Task ShouldUpdateTodoList()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        var command = new UpdateTodoListCommand
        {
            Id = listId,
            Title = "Updated List Title"
        };

        await SendAsync(command);

        var list = await FindAsync<TodoList>(listId);

        list.Should().NotBeNull();
        list!.Title.Should().Be(command.Title);
        list.LastModifiedBy.Should().NotBeNull();
        list.LastModifiedBy.Should().Be(userId);
        list.LastModified.Should().NotBeNull();
        list.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
