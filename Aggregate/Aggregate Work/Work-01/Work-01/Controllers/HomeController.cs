using Microsoft.AspNetCore.Mvc;

namespace Work_01.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
