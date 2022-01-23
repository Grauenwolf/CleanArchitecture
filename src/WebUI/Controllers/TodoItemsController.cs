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

    [HttpPut()]
    public async Task<ActionResult> Update(UpdateTodoItemCommand command)
    {
        await _todoItemService.Update(command);

        return NoContent();
    }

    [HttpPut("[action]")]
    public async Task<ActionResult> UpdateItemDetails(UpdateTodoItemDetailCommand command)
    {
        await _todoItemService.Update(command);


        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _todoItemService.Delete(id);

        return NoContent();
    }
}

