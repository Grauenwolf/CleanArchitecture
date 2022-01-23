
namespace CleanArchitecture.Infrastructure.TodoItems;

public class CreateTodoItemCommand
{
    public int ListId { get; set; }

    public string? Title { get; set; }
}
