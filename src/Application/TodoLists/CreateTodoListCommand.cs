using MediatR;

namespace CleanArchitecture.Application.TodoLists;

public class CreateTodoListCommand : IRequest<int>
{
    public string? Title { get; set; }
}
