# DesharpCore - C# & VB .NET In-line Debugging Tool

[![Latest Stable Version](https://img.shields.io/badge/Stable-v1.0.1-brightgreen.svg?style=plastic)](https://github.com/tomFlidr/desharp/releases)
![.NET Version](https://img.shields.io/badge/.NET-10.0-brightgreen.svg?style=plastic)


---

## Outline
- [**About**](#about)
- [**Installation**](#installation)
- [**Dependency Injection (.NET 10 / .NET Core)**](#dependency-injection-net-10--net-core)
  - [**Registration**](#registration)
  - [**Console Application (.NET 10)**](#console-application-net-10)
  - [**ASP.NET Core MVC 10 Application**](#aspnet-core-mvc-10-application)
- [**Example Todo MVC Application**](#example-todo-mvc-application)
- [**Do Not Miss**](#do-not-miss)
- [**Usage In Code**](#usage-in-code)
  - [**Basic Dumping & Logging Any Structuralized Variables**](#basic-dumping--logging-any-structuralized-variables)
  - [**Basic Dumping & Logging Exceptions**](#basic-dumping--logging-exceptions)
  - [**All Dump Methods**](#all-dump-methods)
  - [**All Log Methods**](#all-log-methods)
  - [**All Other Methods**](#all-other-methods)
- [**Dumps & Logs Outputs**](#dumps--logs-outputs)
- [**What You Can Dump Or Log**](#what-you-can-dump-or-log)
- [**What Is Rendered (Dumps & Logs)**](#what-is-rendered-dumps--logs)
- [**Configuration**](#configuration)
  - [**Configuration Examples**](#configuration-examples)
  - [**Runtime Configuration Options**](#runtime-configuration-options)
  - [**All Configuration Options**](#all-configuration-options)
- [**Visual Studio - Code Snippets**](#visual-studio---code-snippets)


---
## Acknowledgement:

**This is a fork of the wonderful debugging utility Desharp by tomFlidr. I have updated it to work with ASP.Net Core 10.**

## About

**C# & VB .NET debugging utility for .NET 10+ and ASP.NET Core (MVC or Razor Pages):**
- dump or log **any** structuralized variables
- dump or log exceptions with stack trace and inner exceptions

**Dump any variables into:**
- Console window
- Output debug panel in Visual Studio 
- Web response in the floating window
- Html/text log files on HDD

---


## Requirements

- **.NET 10** or later
- **ASP.NET Core MVC** or **Razor Pages** for web applications (`Microsoft.AspNetCore.App` framework)
- ASP.NET Framework (`System.Web`) is **not supported**

## Installation

[DesharpCore on Nuget.org](https://www.nuget.org/packages/DesharpCore/)

Maintainers: see [PUBLISHING.md](PUBLISHING.md) for how to publish a new version to nuget.org.
```nuget
PM> Install-Package DesharpCore
```


---

## Dependency Injection (.NET 10 / .NET Core)

DesharpCore can be registered into the .NET Core / .NET 10 Dependency Injection container using `IServiceCollection` extension methods. Both `IDesharpService` and `IDesharp` interfaces are registered as singletons, providing access to all dumping, logging, timer, assertion, and configuration methods.

### Registration

Register DesharpCore in `IServiceCollection`:

```cs
using Microsoft.Extensions.DependencyInjection;
using DesharpCore;

// Basic registration
services.AddDesharp();

// Or with configuration action
services.AddDesharp(cfg => {
    cfg.Enabled = true;
    cfg.Directory = "~/Logs";
    cfg.Depth = 3;
    cfg.LogFormat = LogFormat.Html;
});

// Or with DebugConfig instance
services.AddDesharp(new DebugConfig {
    Enabled = true,
    Directory = "~/Logs"
});
```

---

### Console Application (.NET 10)

Example using `Microsoft.Extensions.Hosting` and Dependency Injection in a .NET 10 Console Application:

#### `Program.cs`
```cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DesharpCore;

var builder = Host.CreateApplicationBuilder(args);

// Register DesharpCore services
builder.Services.AddDesharp(cfg => {
    cfg.Enabled = true;
    cfg.Directory = "~/Logs";
    cfg.LogFormat = LogFormat.Text;
});

// Register application worker service
builder.Services.AddTransient<AppWorker>();

using var host = builder.Build();

var worker = host.Services.GetRequiredService<AppWorker>();
worker.Run();

public class AppWorker
{
    private readonly IDesharp _desharp;

    public AppWorker(IDesharp desharp)
    {
        _desharp = desharp;
    }

    public void Run()
    {
        var sampleData = new List<object>
        {
            new { Id = 1, Name = "Item A", Price = 29.99m },
            new { Id = 2, Name = "Item B", Price = 49.99m }
        };

        // Dump structured data to console
        _desharp.Dump(sampleData);

        // Measure execution time
        _desharp.Timer("ProcessBatch");

        try
        {
            // Business logic
            _desharp.Log("Batch processing completed successfully.", Level.INFO);
        }
        catch (Exception ex)
        {
            _desharp.Log(ex);
        }

        _desharp.Timer("ProcessBatch");
    }
}
```

---

### ASP.NET Core MVC 10 Application

Example using DesharpCore with Dependency Injection in an ASP.NET Core MVC (.NET 10) application:

#### `Program.cs`
```cs
using DesharpCore;

var builder = WebApplication.CreateBuilder(args);

// Add MVC / Controllers with Views
builder.Services.AddControllersWithViews();

// Register DesharpCore with dependency injection
builder.Services.AddDesharp(cfg => {
    cfg.Enabled = builder.Environment.IsDevelopment();
    cfg.Directory = "~/Logs";
    cfg.LogFormat = LogFormat.Html;
    cfg.Depth = 4;
});

var app = builder.Build();

// DesharpCore middleware - add it as the first middleware
app.UseDesharp();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

#### `Controllers/HomeController.cs`
```cs
using Microsoft.AspNetCore.Mvc;
using DesharpCore;

namespace MyMvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDesharpService _desharp;

        public HomeController(IDesharpService desharp)
        {
            _desharp = desharp;
        }

        public IActionResult Index()
        {
            var model = new
            {
                Title = "Welcome to ASP.NET Core MVC 10",
                ServerTime = DateTime.UtcNow,
                Features = new[] { "Dependency Injection", "Structured Dumping", "File Logging" }
            };

            // Dump variable for debugging
            _desharp.Dump(model);

            // Log information to HDD log files
            _desharp.Log("Index view rendered", Level.INFO);

            return View(model);
        }

        public IActionResult Details(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException($"Invalid ID: {id}");
                }

                var item = new { Id = id, Status = "Active" };
                _desharp.Dump(item);
                return View(item);
            }
            catch (Exception ex)
            {
                // Log exception with full stack trace and diagnostic info
                _desharp.Log(ex);
                _desharp.Dump(ex);
                return View("Error");
            }
        }
    }
}
```
---

### ASP.NET Core Razor Pages 10 Application

```cs
using DesharpCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddDesharp(cfg => cfg.Enabled = builder.Environment.IsDevelopment());

var app = builder.Build();

// DesharpCore middleware - add it as the first middleware
app.UseDesharp();

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();
```

```cs
public class IndexModel : PageModel
{
    private readonly IDesharpService _desharp;
    public IndexModel(IDesharpService desharp) => _desharp = desharp;

    public void OnGet()
    {
        _desharp.Dump(new { Page = "Index" });
    }
}
```

> The debug bar is appended to `text/html` responses. If you use response compression, call `app.UseDesharp()` after `app.UseResponseCompression()`.

---


## Example Todo MVC Application

The solution contains a complete ASP.NET Core 10 MVC sample, [`ExampleTodoMVC`](ExampleTodoMVC), a todo list that showcases DesharpCore. It references the library project directly.

```
dotnet run --project ExampleTodoMVC
```

Open the printed URL (the app runs in the `Development` environment, where DesharpCore is enabled) and use the debug bar at the bottom of the page.

| What | Where |
|---|---|
| Registration, middleware, panels, `Depth`, `SourceLocation` | `ExampleTodoMVC/Program.cs` |
| `Dump`, `Dump` with `DumpOptions`, `Timer`, `Assert`, caught-exception `Log`/`Dump`, validation dumps | `Controllers/TodoController.cs` |
| `IDesharpService` injected into a service, `Log` with levels | `Services/TodoService.cs` |
| One page per feature (`/Debug`) | `Controllers/DebugController.cs` |

**Easy features** (`/Debug`): dumping strings, numbers, lists and models, logging by level, logging/dumping caught exceptions, static `DesharpCore.Debug` API.

**Advanced features** (`/Debug`): uncaught and nested exceptions rendered by the DesharpCore error page, dump options (`Depth`, `MaxLength`, `Return`), `DataTable` and dictionary dumps, timers and assertions, FirePHP browser console logging (`Fire`), `DumpAndDie`, runtime enable/disable, and the `SystemInfo`, `Session` and `Routing` panels.

---

## Usage In Code

### Basic Dumping & Logging Any Structuralized Variables

#### C# Basic Example
```cs
using DesharpCore;
using System.Collections.Generic;

var list = new List<int?>() { 100, 200, null };
Debug.Dump(list);  // print list by Console.WriteLine(); or append into html response as floating window
Debug.Log(list);   // store dumped list in debug.log or debug.html file on HDD
```

#### VB Basic Example
```vb
Imports DesharpCore
Imports System.Collections.Generic

Dim list As New List(Of Int32?)() { 100, 200, null }
Debug.Dump(list)  ' print list by Console.WriteLine(); or append into html response as floating window
Debug.Log(list)   ' store dumped list in debug.log or debug.html file on HDD
```
Dumped result for both languages:
```
[List<Int32?>[3]]
   0: 100 [Int32]
   1: 200 [Int32]
   2: null
```

### Basic Dumping & Logging Exceptions

#### C# Basic Example
```cs
try {
   throw new Exception("Something wrong!");
} catch (Exception e) {
   Debug.Dump(e);  // print exception by Console.WriteLine(); or append into html response as floating window
   Debug.Log(e);   // store dumped exception in exception.log or exception.html file on HDD
}
```

#### VB Basic Example
```vb
Try
   Throw New Exception("Something wrong!")
Catch e As Exception
   Debug.Dump(e)  ' print exception by Console.WriteLine(); or append into html response as floating window
   Debug.Log(e)   ' store dumped exception in exception.log or exception.html file on HDD
End Try
```

Dumped result for both languages:
```
System.Exception (Hash Code: 50632145):
   Message   : Something wrong!
   Time      : 2017-06-10 13:18:07:300
   Process ID: 7972
   Thread ID : 1
   File      : /Program.cs:8
   -------
       4 | namespace ExampleConsole {
       5 |     class Program {
       6 |         static void Main(string[] args) {
       7 |             try {
   ->  8 |                 throw new Exception("Something wrong!");
       9 |             } catch (Exception e) {
      10 |                 Debug.Dump(e);  // print exception by Console.WriteLine(); or append into html response as floating window
      11 |                 Debug.Log(e);   // store dumped exception in exception.log or exception.html file on HDD
      12 |             }
   -------
   Callstack: 
      ExampleConsole.Program.Main(String[] args) /Program.cs 8
```

### All Dump Methods

```cs
/**
 * Dump any values to application output (in web applications into debug bar,  
 * in desktop applications into console or debug output window)
 */
DesharpCore.Debug.Dump(params object[] args);

/**
 * Dump exception instance to application output if output dumping is enabled. It renders:
 * - exception type, exception message and exception hash id
 * - yes/no if exception has been caught or not caught
 * - error file where exception has been thrown
 * - thread call stack
 * All inner exceptions after this exception in the same way.
 */
DesharpCore.Debug.Dump(Exception exception = null, DumpOptions? options = default(DumpOptions?));

/**
 * Dump any values to application output (in web applications into debug bar,  
 * in desktop applications into console or debug output window)
 * This method dumps only single object with dump options like to dump
 * different object depth as usual, different string length or to dump source location and more...
 */
DesharpCore.Debug.Dump(object obj, DumpOptions? options = default(DumpOptions?));

/**
 * Dump any type value to direct application output (not into web request debug
 * bar in web applications!) and stop request/thread (in web applications dump into
 * direct response body, in desktop applications into console or debug output window).
 */
DesharpCore.Debug.DumpAndDie(object obj = null, DumpOptions? options = default(DumpOptions?));
```

### All Log Methods
```cs
/**
 * Log exception instance as dumped string into exceptions.log|exceptions.html file. It stores:
 * - exception type
 * - exception message
 * - if exception has been caught or not caught
 * - exception hash id
 * - error file where exception has been thrown
 * - thread call stack
 * All inner exceptions after this exception in the same way.
 */
DesharpCore.Debug.Log(Exception exception = null);

/**
 * Log any type value to application *.log|*.html file, specified by level param.
 */
DesharpCore.Debug.Log(object obj = null, Level level = Level.INFO, int maxDepth = 0, int maxLength = 0);
```

### All Other Methods
```cs
/**
 * Print to application output or log into file (if enabled) first param to be true
 * or not and describe what was equal or not in first param by second param message.
 */
DesharpCore.Debug.Assert(bool assertion, string description = "", Level logLevel = Level.DEBUG);

/**
 * Configure DesharpCore assembly from running application environment and override
 * any appsettings.json settings or automatically detected settings.
 */
DesharpCore.Debug.Configure(DebugConfig cfg);

/**
 * Enable or disable variables dumping from application code environment for all threads 
 * or get enabled/disabled Desharp dumping state if no boolean param provided.
 */
DesharpCore.Debug.Enabled(bool? enabled = default(bool?));

/**
 * Return last uncaught Exception in request, mostly used in web applications by
 * error page rendering process to know something about Exception before.
 */
DesharpCore.Debug.GetLastError();

/**
 * Return spent request processing time for web applications or return application
 * up time for all other platforms.
 */
DesharpCore.Debug.GetProcessingTime();

/**
 * Print current thread stack trace into application output and exit running application.
 * In web applications - stop current request, in any other applications - stop application
 * with all it's threads and exit.
 */
DesharpCore.Debug.Stop();

/**
 * Prints to output or into log file number of seconds from last timer call under
 * called name in seconds with 3 floating point decimal spaces.
 * If no name specified or name is empty string, there is returned:
 * Web applications - number of seconds from request beginning.
 * Desktop applications - number of seconds from application start.
 */
DesharpCore.Debug.Timer(string name = null, bool returnTimerSeconds = false, Level logLevel = Level.DEBUG);
```

---


## Dumps & Logs Outputs

- console window for console applications
- Visual Studio console for Windows Forms or WPF applications
- floating window in HTML response for web applications
- special HTTP headers for FilePHP browser extension for web applications
- file logs in text/html formats


---


## What You Can Dump Or Log

- any variables
  - primitive variables and its primitive arrays like: `char`, `int?[]` and more 
  - collections: `Lists`, `Dictionaries` or any collections (`IList`, `IDictionary`, `ICollection`, `IEnumerable`...)
  - database results: `DataSet`, `DataTable` and `DataRow` with values
  - formated instances: `DateTimeOffset`, `DateTime`, `TimeSpan`, `Guid`, `StringBuilder`
  - any custom class instances with rendered events targets, property values, and field values
  - anonymous objects like: `new { any = "value" }`
  - `Func<>` and `Delegate` types
  - reflection objects are only displayed as type names
- exceptions
- exceptions with inner exceptions
- much more... you can try:-)

## What Is Rendered (Dumps & Logs)

- variables
  - always is rendered/logged the dumped variable:-)
  - dump is rendered with a configurable return flag (to return a dumped result as string)
  - dump or log is rendered with configurable:
    - source location call (default: `false`)
    - dump depth (default: `3`)
    - maximum string length (default: `1024`)

- exceptions
  - dump or log has always rendered
    - exception message, process, and thread id
    - file and line, where exception has been dumped, logged, or thrown
      (only if source code is possible to target by `*.PDB` file, there is rendered  
      a few lines from source code, where the  `Debug.dump()` or `Debug.Log()` 
	  has been called or where `Exception` has been thrown)
    - exception callstack
      (with editor opener links in web dump or in HTML log)
    - all inner exceptions inside the given exception
  - exceptions in web environment have always rendered:
    - requested URL, all http request headers, and client IP
    - all loaded assemblies
    - there is different dump rendering for catched and not catched exceptions:
      - if the optional flag for catched exception is `true`, exception is rendered as
	     an openable exception in the floating bar in the left bottom screen corner
	  - if the optional flag for catched exception is `false`, exception is rendered 
	    over the whole browser window immediately with the possibility to close it back into the floating bar 


---


## Configuration

You can configure the `DesharpCore` utility by:
- `appsettings.json` or environment variables - more in [**all configuration options**](#all-configuration-options)
- anytime directly by calling `Debug.Configure()` from any thread - more in [**runtime configuration options**](#runtime-configuration-options)

You can configure:
- if dumping/debugging is enabled or not (enabled by default after nuget package installation)
- debug IPs to enable debugging only for listed client IPs (localhost IPs by default)
- logs directory (`~/Logs` by default, relative from app root)
- logs file format:
  - HTML (`*.html`, by default in a config file for all apps)
  - text (`*.log`, by default for desktop apps if the config file is missing)
- logging levels:
  - debug (by default for all variables)
  - exception (by default for logged exceptions)
  - info
  - notice
  - warning
  - error
  - critical
  - alert
  - emergency
  - javascript
- favourite editor to open files from HTML debug output or log file by `editor://` protocol (`MSVS` by default)
- source file location to render where the `DesharpCore` library has been called in your source code


### Configuration Examples

Add the `DesharpCore` section into your `appsettings.json` file:

```json
{
  "DesharpCore": {
    "Enabled": "1",
    "Output": "html",
    "DebugIps": "127.0.0.1,::1",
    "Levels": "exception,debug,info,-notice,-warning,error,critical,alert,emergency,javascript",
    "Panels": "DesharpCore.Panels.SystemInfo,DesharpCore.Panels.Session,DesharpCore.Panels.Routing",
    "Directory": "~/Logs"
  }
}
```

For web applications, add the `DesharpCore` middleware as the first middleware in the request pipeline:

```cs
app.UseDesharp();
```

### Runtime Configuration Options

To overwrite `appsettings.json` settings in a running application or to temporarily define different configuration, 
you can in any application place and in any thread to reconfigure `DesharpCore` utility (for all threads) to call:
```cs
DesharpCore.Debug.Configure(new DesharpCore.DebugConfig {
   Enabled = true,
   SourceLocation = true,
   Directory = "~/Logs",
   LogWriteMilisecond = 10000,
   Depth = 3,
   // `EnvType.Web` or `EnvType.Windows`, used very rarely:
   EnvType = EnvType.Web,
   // `DesharpCore.LogFormat.Html` or `DesharpCore.LogFormat.Text`:
   LogFormat = DesharpCore.LogFormat.Html,
   // for web apps only:
   ErrorPage = "~/custom-error-page.html",
   Panels = new[] { typeof(DesharpCore.Panels.SystemInfo), typeof(DesharpCore.Panels.Session) }
});
```

### All Configuration Options

Add the `DesharpCore` section into your `appsettings.json` (or `appsettings.{Environment}.json`).  
An example file `appsettings.DesharpCore.example.json` is shipped in the nuget package.  
The same keys can be set by environment variables with double underscore, like `DesharpCore__Enabled=1`.  
Here are all detailed configuration options you can copy and paste:


```jsonc
{
  "DesharpCore": {
    
    /* 
      If `true`, all dumps by the method `Debug.Dump()` are internally rendered and displayed.
      For web applications, there is a debug panel that is displayed in a browser with dumped variables or rendered exceptions.
      For desktop applications, there is console priniting enabled or output debug window printing enabled.
      - Not required to configure, but very recomanded.
      - Possible values: `1`, `0`, `true`, `false`.
      - If not configured - enabled is only when the VS debugger is attached or entry assembly is builded as Debug.
      - If disabled, all `Debug.Log()` calls are still enabled and executed, see more option `DesharpCore:Levels`.
    */
    "Enabled": "1",
    
    /*
      Logs content format.
      - Not required, `text` by default.
      - Possible values: `html`, `text`.
    */
    "Output": "html",
    
    /*
      Client IP adresses list to limit `DesharpCore` enabled state only for some clients.
      - Not required to configure, very recomanded for web applications.
      - Possible values: IPv4 or IPv6, separated by comma (IPv6 without `[]` brackets).
      - If not configured and `DesharpCore` is in enabled state, then `DesharpCore` is enabled for all clients.
    */
    "DebugIps": "127.0.0.1,::1",
    
    /*
      Logging levels to enable/disable to write on the hard drive and also the possibility to enable/disable email notifications.
      - Not required, commanded.
      - Possible values: `exception`, `debug`, `info`, `notice`, `warning`, `error`, `critical`, `alert`, `emergency`, `javascript`.
      - If not configured, all logging levels are enabled for logging and not enabled for email notifications.
      - If at least one level is configured, then all other configured levels are disabled for logging and for email notifications.
      - If you want to enable any logging level - put the level name into the node ` value ` attribute (comma separated).
      - If you want to disable any logging level, put minus (-) character before the level name or remove the level name.
      - If you want to enable any logging level for email notifications, put a plus (+) character before the level name.
		For any notification type with a plus sign, it required to configure `DesharpCore:NotifySettings` property!
    */
    "Levels": "+exception,debug,info,-notice,-warning,+error,+critical,alert,+emergency,javascript",

    /*
      Logged messages notifications by configured levels.
      - Not required, commanded in production mode.
      - Possible values: 
        - `host`: Required, mail server smtp domain | IPv4 | IPv6.
        - `port`: Not required, `25` by default.
        - `ssl`: Not required, `false` by default.
        - `from`: Required if no username and password specified, email address to specify sender, if no value specified, there is used `username` value.
        - `username` and `password`: Required if no `from` sender specified, mail server username/password credentials for sender account, always necessary to use together.
		- `domain` - Not required, no default value. Used only as third parametter for `System.Net.NetworkCredential` if presented.
        - `to`: Required, single recepient email adress or multiple adresses separated by semicolon `;`.
        - `priority`: Not required, possible values: `low` | `normal` | `high` (`normal` by defaut).
        - `timeout`: Not required, smtp server timeout specified in miliseconds, `10000` by default (10 seconds).
		- `background`: Not required at all. The default value is `true` to send all notifications in background thread. 
		  Use `false` value only to debug email sending.
    */
    "NotifySettings": "{ host: 'smtp.company.com', port: 587, ssl: true, user: 'noreply@company.com', password: 'your-secret-password', from: 'noreply@company.com', to: 'your.name@gmail.com', priority: 'high', timeout: 30000 }",
    
    /*
      Web debug bar panels.
      - Not required, recomanded.
      - Full class names separated by comma `,`.
      - Panel class has to implement public interface: `DesharpCore.Panels.IPanel`
      - Panel class could implement interface: `DesharpCore.Panels.ISessionPanel`,
        where are method called when session is is read and written every request.
      - There are always enabled build-in panels for execution time, dumps and exceptions.
      - Build-in panels you can optionally use: 
        - `DesharpCore.Panels.SystemInfo` - to display most important request info
        - `DesharpCore.Panels.Session` - to display session id and values (requires `app.UseSession()`)
        - `DesharpCore.Panels.Routing` - to display ASP.NET Core endpoint routes and the matched endpoint
    */
    "Panels": "DesharpCore.Panels.SystemInfo,DesharpCore.Panels.Session",
    
    /*
      Absolute or relative path from the application root directory.
      - Not required, recomanded.
      - Relative path from app root has to start with '~/' like: '~/Path/To/Logs'.
      - If not configured, all log files are written into the application root directory.
    */
    "Directory": "~/Logs",
    
    /* 
      Always render the source location from where the dump has been called by `Debug.Dump()`.
      - Not required, recomanded.
      - Possible values: `1`, `0`, `true`, `false`.
      - If not configured, no dump source locations are rendered in dump ouputs.
    */
    "SourceLocation": "1",
    
    /*
      Milisecond timeout how often logged messages or exceptions are written from memory to hard drive.
      - Not required, recomanded for speed optimalization in production mode.
      - Possible values - use digits to define any miliseconds integer value.
      - If not configured, all messages or exceptions are written immediately 
        in the current thread, which is called `DesharpCore.Log()`.
    */
    "WriteMiliseconds": "5000",
    
    /*
      .NET objects dumping depth.
      - Not required, recomanded for speed optimalization in production mode.
      - Possible values: just digits like `2`, `3`, `4` or `5`.
      - If not configured, `3` by default.
    */
    "Depth": "3",
    
    /*
      Maximum length for dumped string values.
      - Not required, recomanded for speed optimalization in production mode.
      - Possible values: just digits like `512`, `1024` or `5000`...
      - If not configured, `1024` by default.
    */
    "MaxLength": "1024",

    /*
      Custom web error page.
      - Not required, recomanded for production mode.
      - If `DesharpCore` is not enabled and there is an uncaught exception in your application,
        you can use a custom static error page to transfer into the client browser.
      - If not configured, the ` DesharpCore ` built-in error page is used by default with error 500.
    */
    "ErrorPage": "~/custom-error-page-500.html",

    /*
      Dump all internal .NET events, property ad fields in custom class instances, 
      (created internally and usually for property values) and all class instance 
      member types marked with the ` System.Runtime.CompilerServices.CompillerGenerated`
		attribute or with the `DesharpCore.Hidden` attribute.
      This option is useful to see everything in memory.
      - Not required, commanded only for deep development view.
      - Possible values: `1`, `0`, `true`, `false`.
      - If not configured, `false` by default.
      - If not configured or configured as `false`, you can hide 
        your class members for dumping and logging by attributes:
        - [System.Runtime.CompilerServices.CompilerGenerated] - .NET standard.
        - [DesharpCore.Hidden] - shorter.
    */
    "DumpCompillerGenerated": "true",
    
    /* 
      Default editor param value.
      - Not required, marginal.
      - For automatic file opening in Visual Studio or in any other editor by rendered links:
        `<a href="editor://file=...&line=...&editor=MSVS2019">../File.cs:123</a>
      - Possible values: any string key to open your editor from HTML output by.
      - If not configured, the value is automatically detected by Visual Studio installation on the current system.
    */
    "Editor": "MSVS2019"
  }
}
```

---


## Visual Studio – Code Snippets

To use long **[DesharpCore](https://www.nuget.org/packages/DesharpCore/)** calls more comfortably, install [Visual Studio code snippets](https://github.com/rsobrepena/desharpcore-codesnippets) for desharpcore to create proper shortcuts
More info [about code snippets for Visual Studio](https://code.visualstudio.com/docs/editor/userdefinedsnippets).

