using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class WaiterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
