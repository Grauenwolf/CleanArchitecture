using AutoMapper;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.TodoLists;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CleanArchitecture.Application.IntegrationTests.TodoLists.Commands;

using static Testing;

public class DeleteTodoListTests : TestBase
{
    [Test]
    public async Task ShouldRequireValidTodoListId()
    {
        //Setup
        var userId = await RunAsDefaultUserAsync();
        using var scope = ScopeFactory.CreateScope();
        var service = new TodoListService(
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
            scope.ServiceProvider.GetRequiredService<IMapper>(),
            scope.ServiceProvider.GetRequiredService<ICsvFileBuilder>()
            );

        var command = new DeleteTodoListCommand { Id = 99 };
        await FluentActions.Invoking(() => service.Delete(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ShouldDeleteTodoList()
    {
        //Setup
        var userId = await RunAsDefaultUserAsync();
        using var scope = ScopeFactory.CreateScope();
        var service = new TodoListService(
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
            scope.ServiceProvider.GetRequiredService<IMapper>(),
            scope.ServiceProvider.GetRequiredService<ICsvFileBuilder>()
            );


        var listId = await service.Create(new CreateTodoListCommand
        {
            Title = "New List"
        });

        await service.Delete(new DeleteTodoListCommand
        {
            Id = listId
        });

        var list = await FindAsync<TodoList>(listId);

        list.Should().BeNull();
    }
}
