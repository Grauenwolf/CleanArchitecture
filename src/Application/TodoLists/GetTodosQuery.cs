using MediatR;

namespace CleanArchitecture.Application.TodoLists;

public class GetTodosQuery : IRequest<TodosVm>
{
}
