using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Infrastructure.Enums;
using CleanArchitecture.Infrastructure.Exceptions;
using CleanArchitecture.Infrastructure.Files;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.TodoLists;

public class TodoListService
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICsvFileBuilder _fileBuilder;

    public TodoListService(IApplicationDbContext context, IMapper mapper, ICsvFileBuilder fileBuilder)
    {
        _context = context;
        _mapper = mapper;
        _fileBuilder = fileBuilder;
    }

    public async Task Update(UpdateTodoListCommand request)
    {
        var entity = await _context.TodoLists.FindAsync(request.Id);

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoList), request.Id);
        }

        entity.Title = request.Title;

        await _context.SaveChangesAsync();

        return;
    }

    public async Task<TodosVm> Get(CancellationToken cancellationToken)
    {
        return new TodosVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new PriorityLevelDto { Value = (int)p, Name = p.ToString() })
                .ToList(),

            Lists = await _context.TodoLists
                .AsNoTracking()
                .ProjectTo<TodoListDto>(_mapper.ConfigurationProvider)
                .OrderBy(t => t.Title)
                .ToListAsync(cancellationToken)
        };
    }

    public async Task<ExportTodosVm> Export(ExportTodosQuery request, CancellationToken cancellationToken)
    {
        var records = await _context.TodoItems
                .Where(t => t.ListId == request.ListId)
                .ProjectTo<TodoItemRecord>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

        var vm = new ExportTodosVm(
            "TodoItems.csv",
            "text/csv",
            _fileBuilder.BuildTodoItemsFile(records));

        return vm;
    }

    public async Task<int> Create(CreateTodoListCommand request)
    {
        var entity = new TodoList();

        entity.Title = request.Title;

        _context.TodoLists.Add(entity);

        await _context.SaveChangesAsync();

        return entity.Id;
    }

    public async Task Delete(int id)
    {
        var entity = await _context.TodoLists
            .Where(l => l.Id == id)
            .SingleOrDefaultAsync();

        if (entity == null)
        {
            throw new NotFoundException(nameof(TodoList), id);
        }

        _context.TodoLists.Remove(entity);

        await _context.SaveChangesAsync();

        return;
    }
}
