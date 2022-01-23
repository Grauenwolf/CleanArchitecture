using MediatR;

namespace CleanArchitecture.Application.TodoItems;

public class CreateTodoItemCommand : IRequest<int>
{
    public int ListId { get; set; }

    public string? Title { get; set; }
}
