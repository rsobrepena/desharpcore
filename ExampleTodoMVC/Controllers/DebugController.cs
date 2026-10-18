using System.Data;
using DesharpCore;
using ExampleTodoMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExampleTodoMVC.Controllers;

/// <summary>One action per DesharpCore feature. Static <c>Debug</c> and injected <c>IDesharpService</c> do the same.</summary>
public class DebugController : Controller
{
    private readonly ITodoService _todos;
    private readonly IDesharpService _desharp;

    public DebugController(ITodoService todos, IDesharpService desharp)
    {
        _todos = todos;
        _desharp = desharp;
    }

    public IActionResult Index() => View();

    // Easy: dump anything into the debug bar
    public IActionResult Dump()
    {
        _desharp.Dump("a string");
        _desharp.Dump(42, 3.14m, DateTime.Now, Guid.NewGuid());
        _desharp.Dump(new List<int?> { 1, 2, null });
        _desharp.Dump(_todos.All());
        return RedirectToAction("Index", "Todo");
    }

    // Easy: logs written by level (*.log files in the app directory)
    public IActionResult Log()
    {
        _desharp.Log("Plain info message");                       // Level.INFO
        _desharp.Log(new { User = "demo" }, Level.DEBUG);
        _desharp.Log("Careful!", Level.WARNING);
        _desharp.Log("Something failed", Level.ERROR);
        return RedirectToAction("Index", "Todo");
    }

    // Easy: exceptions
    public IActionResult CaughtException()
    {
        try { throw new InvalidOperationException("Caught on purpose"); }
        catch (Exception ex) { _desharp.Log(ex); _desharp.Dump(ex); }
        return RedirectToAction("Index", "Todo");
    }

    // Advanced: uncaught exception rendered by the DesharpCore error page
    public IActionResult Throw() => throw new InvalidOperationException("Uncaught exception from DebugController.Throw()");

    // Advanced: nested exception with an inner exception
    public IActionResult NestedException()
    {
        try
        {
            try { _ = int.Parse("not a number"); }
            catch (Exception inner) { throw new ApplicationException("Outer failure", inner); }
        }
        catch (Exception ex) { throw new Exception("Wrapper failure", ex); }
        return Ok();
    }

    // Advanced: dump options - depth, max length, returned string
    public IActionResult DumpOptions()
    {
        var deep = new { L1 = new { L2 = new { L3 = new { L4 = new { L5 = "deep" } } } }, Long = new string('x', 2000) };
        _desharp.Dump(deep, new DumpOptions { Depth = 2, MaxLength = 100 });
        string text = _desharp.Dump(deep, new DumpOptions { Depth = 6, Return = true });
        return Content(text, "text/plain");
    }

    // Advanced: dump DataTable, dictionary, exception data
    public IActionResult DataTypes()
    {
        var table = new DataTable("Todos");
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Title", typeof(string));
        foreach (var t in _todos.All()) table.Rows.Add(t.Id, t.Title);
        _desharp.Dump(table);
        _desharp.Dump(new Dictionary<string, object> { ["now"] = DateTime.UtcNow, ["id"] = Guid.NewGuid(), ["price"] = 123.45m });
        return RedirectToAction("Index", "Todo");
    }

    // Advanced: timers and assertions
    public IActionResult Timers()
    {
        _desharp.Timer("work");
        Thread.Sleep(120);
        _desharp.Timer("work");
        double seconds = _desharp.Timer("work", returnTimerSeconds: true);
        _desharp.Assert(seconds > 0.1, "Work took longer than 100 ms");
        _desharp.Assert(false, "This assertion fails on purpose", Level.WARNING);
        _desharp.Dump(new { seconds, requestSeconds = _desharp.GetProcessingTime() });
        return RedirectToAction("Index", "Todo");
    }

    // Advanced: FirePHP-style browser console logging
    public IActionResult Fire()
    {
        _desharp.Fire.Info("Fire info").Warn("Fire warning").Table("Todos", _todos.All());
        return RedirectToAction("Index", "Todo");
    }

    // Advanced: DumpAndDie - dump into the response body and stop the request
    public IActionResult Die()
    {
        _desharp.DumpAndDie(_todos.All());
        return Ok(); // never reached
    }

    // Advanced: runtime enable/disable
    public IActionResult Toggle()
    {
        bool enabled = _desharp.Enabled(!_desharp.Enabled());
        TempData["Message"] = "DesharpCore is now " + (enabled ? "enabled" : "disabled");
        return RedirectToAction("Index");
    }

    // Static API: same as the injected service
    public IActionResult StaticApi()
    {
        DesharpCore.Debug.Dump("Dumped by the static DesharpCore.Debug class");
        DesharpCore.Debug.Log("Logged by the static DesharpCore.Debug class", Level.INFO);
        return RedirectToAction("Index", "Todo");
    }
}
