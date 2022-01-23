
namespace CleanArchitecture.Infrastructure.TodoLists;

public class UpdateTodoListCommand
{
    public int Id { get; set; }

    public string? Title { get; set; }
}
