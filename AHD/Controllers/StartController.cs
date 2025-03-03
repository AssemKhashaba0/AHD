using Microsoft.AspNetCore.Mvc;

namespace AHD.Controllers
{
    public class StartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
