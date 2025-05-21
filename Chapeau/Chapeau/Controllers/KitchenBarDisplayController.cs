using Chapeau.Services;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Models;
using Chapeau.Enums;

namespace Chapeau.Controllers
{
    //pato
    public class KitchenBarDisplayController : Controller
    {
        private readonly IKitchenBarDisplayService _kitchenBarDisplaySevice;

        public KitchenBarDisplayController(IKitchenBarDisplayService kitchenBarDisplayServices)
        {
            _kitchenBarDisplaySevice = kitchenBarDisplayServices;
        }

        public IActionResult Index(Status? status)
        {
            // Get the orders from the service
            List<DisplayOrder> orders = _kitchenBarDisplaySevice.GetOrders(status);
            return View(orders);
        }
    }
}
