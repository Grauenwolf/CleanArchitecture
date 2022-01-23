using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebUI.Controllers;

[Authorize]
public class TodoItemsController : ApiControllerBase
{
    readonly GetTodoItemsWithPaginationQueryHandler _getTodoItemsWithPaginationQueryHandler;
    readonly CreateTodoItemCommandHandler _createTodoItemCommandHandler;
    readonly UpdateTodoItemCommandHandler _updateTodoItemCommandHandler;
    readonly UpdateTodoItemDetailCommandHandler _updateTodoItemDetailCommandHandler;
    readonly DeleteTodoItemCommandHandler _deleteTodoItemCommandHandler;



    public TodoItemsController(
        GetTodoItemsWithPaginationQueryHandler getTodoItemsWithPaginationQueryHandler,
        CreateTodoItemCommandHandler createTodoItemCommandHandler,
        UpdateTodoItemCommandHandler updateTodoItemCommandHandler,
        UpdateTodoItemDetailCommandHandler updateTodoItemDetailCommandHandler,
        DeleteTodoItemCommandHandler deleteTodoItemCommandHandler

        )
    {
        _deleteTodoItemCommandHandler = deleteTodoItemCommandHandler;
        _updateTodoItemDetailCommandHandler = updateTodoItemDetailCommandHandler;
        _updateTodoItemCommandHandler = updateTodoItemCommandHandler;
        _createTodoItemCommandHandler = createTodoItemCommandHandler;
        _getTodoItemsWithPaginationQueryHandler = getTodoItemsWithPaginationQueryHandler;

    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<TodoItemBriefDto>>> GetTodoItemsWithPagination([FromQuery] GetTodoItemsWithPaginationQuery query)
    {
        return await _getTodoItemsWithPaginationQueryHandler.Handle(query, CancellationToken.None);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTodoItemCommand command)
    {
        return await _createTodoItemCommandHandler.Handle(command, CancellationToken.None);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateTodoItemCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _updateTodoItemCommandHandler.Handle(command, CancellationToken.None);

        return NoContent();
    }

    [HttpPut("[action]")]
    public async Task<ActionResult> UpdateItemDetails(int id, UpdateTodoItemDetailCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _updateTodoItemDetailCommandHandler.Handle(command, CancellationToken.None);


        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _deleteTodoItemCommandHandler.Handle(new DeleteTodoItemCommand { Id = id }, CancellationToken.None);

        return NoContent();
    }
}

