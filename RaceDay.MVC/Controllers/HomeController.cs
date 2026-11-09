using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace RaceDay.MVC.Controllers
{
    public class HomeController : Controller
    {
        [Authorize] // Requires login
        public IActionResult Index()
        {
            return View();
        }
    }
}