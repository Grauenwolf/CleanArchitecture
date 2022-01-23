using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.TodoItems;
using CleanArchitecture.Application.TodoLists;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CleanArchitecture.Application.IntegrationTests.TodoItems.Commands;

using static Testing;

public class DeleteTodoItemTests : TestBase
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        //Setup
        var userId = await RunAsDefaultUserAsync();
        using var scope = ScopeFactory.CreateScope();
        var service = new TodoItemService(
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
            scope.ServiceProvider.GetRequiredService<IMapper>()
            );

        var command = new DeleteTodoItemCommand { Id = 99 };

        await FluentActions.Invoking(() =>
            service.Delete(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteTodoItem()
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
        });

        var itemId = await service2.Create(new CreateTodoItemCommand
        {
            ListId = listId,
            Title = "New Item"
        });

        await service2.Delete(new DeleteTodoItemCommand
        {
            Id = itemId
        });

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().BeNull();
    }
}
