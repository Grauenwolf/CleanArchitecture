using CleanArchitecture.Infrastructure.Enums;

namespace CleanArchitecture.Infrastructure.Persistence.Entities;

public class TodoItem : AuditableEntity
{
    public int Id { get; set; }

    public int ListId { get; set; }

    public string? Title { get; set; }

    public string? Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }
    public bool Done { get; set; }

    public TodoList List { get; set; } = null!;

}
