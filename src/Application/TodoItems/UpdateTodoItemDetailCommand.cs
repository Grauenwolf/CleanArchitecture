using CleanArchitecture.Domain.Enums;
using MediatR;

namespace CleanArchitecture.Application.TodoItems;

public class UpdateTodoItemDetailCommand : IRequest
{
    public int Id { get; set; }

    public int ListId { get; set; }

    public PriorityLevel Priority { get; set; }

    public string? Note { get; set; }
}
