using MediatR;

namespace CleanArchitecture.Application.TodoLists;

public class ExportTodosQuery : IRequest<ExportTodosVm>
{
    public int ListId { get; set; }
}
