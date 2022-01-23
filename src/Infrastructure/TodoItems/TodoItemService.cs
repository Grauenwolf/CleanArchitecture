using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Infrastructure.Exceptions;
using CleanArchitecture.Infrastructure.Mappings;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Persistence.Entities;

namespace CleanArchitecture.Infrastructure.TodoItems;

public class TodoItemService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TodoItemService(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TodoItemBriefDto>> Get(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        return await _context.TodoItems
            .Where(x => x.ListId == request.ListId)
            .OrderBy(x => x.Title)
            .ProjectTo<TodoItemBriefDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }

    public async Task Delete(int id)
    {
        var entity = await _context.TodoItems.FindAsync(id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), id);
        }

        _context.TodoItems.Remove(entity);

        await _context.SaveChangesAsync();

        return;
    }

    public async Task<int> Create(CreateTodoItemCommand request)
    {
        var entity = new TodoItem
        {
            ListId = request.ListId,
            Title = request.Title,
            Done = false
        };

        _context.TodoItems.Add(entity);

        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task Update(UpdateTodoItemCommand request)
    {
        var entity = await _context.TodoItems.FindAsync(request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        entity.Title = request.Title;
        entity.Done = request.Done;

        await _context.SaveChangesAsync();

        return;
    }

    public async Task Update(UpdateTodoItemDetailCommand request)
    {
        var entity = await _context.TodoItems.FindAsync(request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        entity.ListId = request.ListId;
        entity.Priority = request.Priority;
        entity.Note = request.Note;

        await _context.SaveChangesAsync();

        return;
    }
}
