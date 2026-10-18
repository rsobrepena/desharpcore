using DesharpCore;
using ExampleTodoMVC.Models;
using ExampleTodoMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExampleTodoMVC.Controllers;

public class TodoController : Controller
{
    private readonly ITodoService _todos;
    private readonly IDesharpService _desharp;

    public TodoController(ITodoService todos, IDesharpService desharp)
    {
        _todos = todos;
        _desharp = desharp;
    }

    public IActionResult Index()
    {
        _desharp.Timer("index");                       // start a timer
        var items = _todos.All();
        _desharp.Dump(items);                          // dump into the debug bar
        _desharp.Dump(new { Total = items.Count, Done = items.Count(i => i.IsDone) },
            new DumpOptions { Depth = 2 });             // dump with options
        HttpContext.Session.SetInt32("visits", (HttpContext.Session.GetInt32("visits") ?? 0) + 1);
        _desharp.Assert(items.Count > 0, "There is at least one todo");
        _desharp.Timer("index");                       // print elapsed time
        return View(items);
    }

    public IActionResult Create() => View(new TodoItem());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(TodoItem item)
    {
        if (!ModelState.IsValid)
        {
            // dump validation errors
            _desharp.Dump(ModelState.Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(e => e.Key, e => e.Value.Errors.Select(x => x.ErrorMessage).ToArray()));
            return View(item);
        }
        _todos.Add(item);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var item = _todos.Get(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(TodoItem item)
    {
        if (!ModelState.IsValid) return View(item);
        _todos.Update(item);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Toggle(int id)
    {
        try { _todos.Toggle(id); }
        catch (KeyNotFoundException ex)
        {
            _desharp.Log(ex);                          // log a caught exception
            _desharp.Dump(ex);                         // and dump it into the bar
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _todos.Delete(id);
        return RedirectToAction(nameof(Index));
    }
}
