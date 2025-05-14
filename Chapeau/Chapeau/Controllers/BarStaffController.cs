using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class BarStaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
