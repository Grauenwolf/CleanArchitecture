using CleanArchitecture.Application.TodoLists;

namespace CleanArchitecture.Application.Common.Interfaces;

public interface ICsvFileBuilder
{
    byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
}
