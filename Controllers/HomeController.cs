using Microsoft.AspNetCore.Mvc;

namespace Bake_Link.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Error()
    {
        return View();
    }
}
