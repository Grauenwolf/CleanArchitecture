using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.TodoItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebUI.Controllers;

[Authorize]
public class TodoItemsController : ApiControllerBase
{
    readonly TodoItemService _todoItemService;

    public TodoItemsController(TodoItemService todoItemService)
    {
        _todoItemService = todoItemService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<TodoItemBriefDto>>> GetTodoItemsWithPagination([FromQuery] GetTodoItemsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        return await _todoItemService.Get(query, cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTodoItemCommand command)
    {
        return await _todoItemService.Create(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateTodoItemCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _todoItemService.Update(command);

        return NoContent();
    }

    [HttpPut("[action]")]
    public async Task<ActionResult> UpdateItemDetails(int id, UpdateTodoItemDetailCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _todoItemService.Update(command);


        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _todoItemService.Delete(new DeleteTodoItemCommand { Id = id });

        return NoContent();
    }
}

