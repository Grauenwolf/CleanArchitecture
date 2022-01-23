using CleanArchitecture.Infrastructure.TodoLists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebUI.Controllers;

[Authorize]
public class TodoListsController : ApiControllerBase
{
    readonly TodoListService _todoListService;

    public TodoListsController(TodoListService todoListService)
    {
        _todoListService = todoListService;
    }


    [HttpGet]
    public async Task<ActionResult<TodosVm>> Get(CancellationToken cancellationToken)
    {
        return await _todoListService.Get(cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<FileResult> Get(int id, CancellationToken cancellationToken)
    {
        var vm = await _todoListService.Export(new ExportTodosQuery { ListId = id }, cancellationToken);

        return File(vm.Content, vm.ContentType, vm.FileName);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTodoListCommand command)
    {
        return await _todoListService.Create(command);
    }

    [HttpPut()]
    public async Task<ActionResult> Update(UpdateTodoListCommand command)
    {
        await _todoListService.Update(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _todoListService.Delete(id);

        return NoContent();
    }
}
