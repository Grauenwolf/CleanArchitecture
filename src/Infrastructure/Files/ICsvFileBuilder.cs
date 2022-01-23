using CleanArchitecture.Infrastructure.TodoLists;

namespace CleanArchitecture.Infrastructure.Files;

public interface ICsvFileBuilder
{
    byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
}
