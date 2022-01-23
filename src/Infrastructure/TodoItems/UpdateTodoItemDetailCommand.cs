using CleanArchitecture.Infrastructure.Enums;

namespace CleanArchitecture.Infrastructure.TodoItems;

public class UpdateTodoItemDetailCommand
{
    public int Id { get; set; }

    public int ListId { get; set; }

    public PriorityLevel Priority { get; set; }

    public string? Note { get; set; }
}
