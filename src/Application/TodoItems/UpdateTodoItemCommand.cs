using MediatR;

namespace CleanArchitecture.Application.TodoItems;

public class UpdateTodoItemCommand : IRequest
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public bool Done { get; set; }
}
