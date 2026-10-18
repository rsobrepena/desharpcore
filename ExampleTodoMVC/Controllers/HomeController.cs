using Microsoft.AspNetCore.Mvc;

namespace ExampleTodoMVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Error() => Content("Something went wrong (production error page).");
}
