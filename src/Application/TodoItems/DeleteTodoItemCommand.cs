using MediatR;

namespace CleanArchitecture.Application.TodoItems;

public class DeleteTodoItemCommand : IRequest
{
    public int Id { get; set; }
}
