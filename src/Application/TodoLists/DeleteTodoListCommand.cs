using MediatR;

namespace CleanArchitecture.Application.TodoLists;

public class DeleteTodoListCommand : IRequest
{
    public int Id { get; set; }
}
