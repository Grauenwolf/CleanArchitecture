namespace CleanArchitecture.Infrastructure.TodoItems;

public class GetTodoItemsWithPaginationQuery
{
    public int ListId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
