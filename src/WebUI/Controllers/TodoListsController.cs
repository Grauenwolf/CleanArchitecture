using CleanArchitecture.Application.TodoLists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebUI.Controllers;

[Authorize]
public class TodoListsController : ApiControllerBase
{
    readonly GetTodosQueryHandler _getTodosQueryHandler;
    readonly ExportTodosQueryHandler _exportTodosQueryHandler;
    readonly CreateTodoListCommandHandler _createTodoListCommandHandler;
    readonly UpdateTodoListCommandHandler _updateTodoListCommandHandler;
    readonly DeleteTodoListCommandHandler _deleteTodoListCommandHandler;

    public TodoListsController(
        GetTodosQueryHandler getTodosQueryHandler,
        ExportTodosQueryHandler exportTodosQueryHandler,
        CreateTodoListCommandHandler createTodoListCommandHandler,
        UpdateTodoListCommandHandler updateTodoListCommandHandler,
        DeleteTodoListCommandHandler deleteTodoListCommandHandler
        )
    {
        _getTodosQueryHandler = getTodosQueryHandler;
        _exportTodosQueryHandler = exportTodosQueryHandler;
        _createTodoListCommandHandler = createTodoListCommandHandler;
        _updateTodoListCommandHandler = updateTodoListCommandHandler;
        _deleteTodoListCommandHandler = deleteTodoListCommandHandler;
    }


    [HttpGet]
    public async Task<ActionResult<TodosVm>> Get()
    {
        return await _getTodosQueryHandler.Handle(new GetTodosQuery(), CancellationToken.None);
    }

    [HttpGet("{id}")]
    public async Task<FileResult> Get(int id)
    {
        var vm = await _exportTodosQueryHandler.Handle(new ExportTodosQuery { ListId = id }, CancellationToken.None);

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTodoListCommand command)
    {
        return await _createTodoListCommandHandler.Handle(command, CancellationToken.None);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateTodoListCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _updateTodoListCommandHandler.Handle(command, CancellationToken.None);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _deleteTodoListCommandHandler.Handle(new DeleteTodoListCommand { Id = id }, CancellationToken.None);

        return NoContent();
    }
}
