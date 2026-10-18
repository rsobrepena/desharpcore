using DesharpCore;
using ExampleTodoMVC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddSingleton<ITodoService, TodoService>();

// DesharpCore registration (dependency injection) with the advanced options
builder.Services.AddDesharp(cfg =>
{
    cfg.Enabled = builder.Environment.IsDevelopment();
    cfg.LogFormat = LogFormat.Html;
    cfg.Depth = 4;
    cfg.SourceLocation = true;                // show file:line of each dump
    cfg.Panels = new[]                        // bar panels
    {
        typeof(DesharpCore.Panels.SystemInfo),
        typeof(DesharpCore.Panels.Session),
        typeof(DesharpCore.Panels.Routing)
    };
});

var app = builder.Build();

// DesharpCore middleware - first in the pipeline
app.UseDesharp();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllerRoute("default", "{controller=Todo}/{action=Index}/{id?}");

app.Run();
