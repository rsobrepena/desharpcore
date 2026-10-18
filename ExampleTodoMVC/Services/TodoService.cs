using System.Collections.Concurrent;
using DesharpCore;
using ExampleTodoMVC.Models;

namespace ExampleTodoMVC.Services;

public interface ITodoService
{
    IReadOnlyList<TodoItem> All();
    TodoItem Get(int id);
    TodoItem Add(TodoItem item);
    void Update(TodoItem item);
    void Toggle(int id);
    void Delete(int id);
}

/// <summary>In-memory todo store. Shows IDesharpService used from a service (dependency injection).</summary>
public class TodoService : ITodoService
{
    private readonly ConcurrentDictionary<int, TodoItem> _items = new();
    private readonly IDesharpService _desharp;
    private int _nextId;

    public TodoService(IDesharpService desharp)
    {
        _desharp = desharp;
        Add(new TodoItem { Title = "Install DesharpCore", Priority = Priority.High, IsDone = true });
        Add(new TodoItem { Title = "Open the debug bar", Notes = "Look at the bar at the bottom of the page.", Priority = Priority.Normal });
        Add(new TodoItem { Title = "Trigger an exception", Notes = "Use the Debug menu.", Priority = Priority.Low, DueDate = DateTime.Today.AddDays(3) });
    }

    public IReadOnlyList<TodoItem> All() => _items.Values.OrderBy(i => i.IsDone).ThenByDescending(i => i.Priority).ThenBy(i => i.Id).ToList();

    public TodoItem Get(int id) => _items.TryGetValue(id, out var item) ? item : null;

    public TodoItem Add(TodoItem item)
    {
        item.Id = Interlocked.Increment(ref _nextId);
        _items[item.Id] = item;
        _desharp.Log(item, Level.INFO);
        return item;
    }

    public void Update(TodoItem item)
    {
        _items[item.Id] = item;
        _desharp.Log($"Todo #{item.Id} updated", Level.NOTICE);
    }

    public void Toggle(int id)
    {
        var item = Get(id) ?? throw new KeyNotFoundException($"Todo #{id} does not exist.");
        item.IsDone = !item.IsDone;
        _desharp.Log($"Todo #{id} marked {(item.IsDone ? "done" : "open")}", Level.INFO);
    }

    public void Delete(int id)
    {
        if (!_items.TryRemove(id, out var removed))
            _desharp.Log($"Delete of missing todo #{id}", Level.WARNING);
        else
            _desharp.Log(removed, Level.WARNING);
    }
}
