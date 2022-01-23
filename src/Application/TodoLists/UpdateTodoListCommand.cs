using MediatR;

namespace CleanArchitecture.Application.TodoLists;

public class UpdateTodoListCommand : IRequest
{
    public int Id { get; set; }

    public string? Title { get; set; }
}
