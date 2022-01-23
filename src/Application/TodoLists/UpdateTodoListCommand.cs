
namespace CleanArchitecture.Application.TodoLists;

public class UpdateTodoListCommand
{
    public int Id { get; set; }

    public string? Title { get; set; }
}
