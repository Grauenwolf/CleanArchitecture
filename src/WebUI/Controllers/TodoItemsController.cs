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
    public async Task<ActionResult<PaginatedList<TodoItemBriefDto>>> GetTodoItemsWithPagination([FromQuery] GetTodoItemsWithPaginationQuery query)
    {
        return await _todoItemService.Get(query, CancellationToken.None);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTodoItemCommand command)
    {
        return await _todoItemService.Create(command, CancellationToken.None);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateTodoItemCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _todoItemService.Update(command, CancellationToken.None);

        return NoContent();
    }

    [HttpPut("[action]")]
    public async Task<ActionResult> UpdateItemDetails(int id, UpdateTodoItemDetailCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _todoItemService.Update(command, CancellationToken.None);


        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _todoItemService.Delete(new DeleteTodoItemCommand { Id = id }, CancellationToken.None);

        return NoContent();
    }
}

