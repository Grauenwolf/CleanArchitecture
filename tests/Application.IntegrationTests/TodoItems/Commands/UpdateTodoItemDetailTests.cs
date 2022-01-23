using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.TodoItems;
using CleanArchitecture.Application.TodoLists;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CleanArchitecture.Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class UpdateTodoItemDetailTests : TestBase
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        //Setup
        var userId = await RunAsDefaultUserAsync();
        using var scope = ScopeFactory.CreateScope();
        var service1 = new TodoListService(
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
            scope.ServiceProvider.GetRequiredService<IMapper>(),
            scope.ServiceProvider.GetRequiredService<ICsvFileBuilder>()
            );
        var service2 = new TodoItemService(
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
            scope.ServiceProvider.GetRequiredService<IMapper>()
            );

        var command = new UpdateTodoItemCommand { Id = 99, Title = "New Title" };
        await FluentActions.Invoking(() => service2.Update(command, CancellationToken.None)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldUpdateTodoItem()
    {
        //Setup
        var userId = await RunAsDefaultUserAsync();
        using var scope = ScopeFactory.CreateScope();
        var service1 = new TodoListService(
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
            scope.ServiceProvider.GetRequiredService<IMapper>(),
            scope.ServiceProvider.GetRequiredService<ICsvFileBuilder>()
            );
        var service2 = new TodoItemService(
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
            scope.ServiceProvider.GetRequiredService<IMapper>()
            );


        var listId = await service1.Create(new CreateTodoListCommand
        {
            Title = "New List"
        }, CancellationToken.None);

        var itemId = await service2.Create(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        }, CancellationToken.None);

        var command = new UpdateTodoItemDetailCommand
        {
            Id = itemId,
            ListId = listId,
            Note = "This is the note.",
            Priority = PriorityLevel.High
        };

        await service2.Update(command, CancellationToken.None);

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.ListId.Should().Be(command.ListId);
        item.Note.Should().Be(command.Note);
        item.Priority.Should().Be(command.Priority);
        item.LastModifiedBy.Should().NotBeNull();
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().NotBeNull();
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
