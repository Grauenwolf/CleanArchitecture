using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Events;
using MediatR;

namespace CleanArchitecture.Application.TodoItems;

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

    public async Task<Unit> Delete(DeleteTodoItemCommand request)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { request.Id });

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        _context.TodoItems.Remove(entity);

        entity.DomainEvents.Add(new TodoItemDeletedEvent(entity));

        await _context.SaveChangesAsync();

        return Unit.Value;
    }

    public async Task<int> Create(CreateTodoItemCommand request)
    {
        var entity = new TodoItem
        {
            ListId = request.ListId,
            Title = request.Title,
            Done = false
        };

        entity.DomainEvents.Add(new TodoItemCreatedEvent(entity));

        _context.TodoItems.Add(entity);

        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<Unit> Update(UpdateTodoItemCommand request)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { request.Id });

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        entity.Title = request.Title;
        entity.Done = request.Done;

        await _context.SaveChangesAsync();

        return Unit.Value;
    }

    public async Task<Unit> Update(UpdateTodoItemDetailCommand request)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { request.Id });

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoItem), request.Id);
        }

        entity.ListId = request.ListId;
        entity.Priority = request.Priority;
        entity.Note = request.Note;

        await _context.SaveChangesAsync();

        return Unit.Value;
    }
}
