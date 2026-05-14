using Microsoft.AspNetCore.Mvc;

namespace EmptyMvcProject.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Content("Hello World");
    }
}
