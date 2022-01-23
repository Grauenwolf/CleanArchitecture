using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.TodoItems;

public class UpdateTodoItemDetailCommand
{
    public int Id { get; set; }

    public int ListId { get; set; }

    public PriorityLevel Priority { get; set; }

    public string? Note { get; set; }
}
