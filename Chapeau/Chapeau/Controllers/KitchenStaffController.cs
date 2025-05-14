using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class KitchenStaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
