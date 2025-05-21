using Microsoft.AspNetCore.Mvc;
using Chapeau.Repositories;
using Chapeau.Models;

namespace Chapeau.Controllers
{
    public class MenuItemController : Controller
    {
        private readonly IMenuItemsRepository _menuItemsRepository;

        public MenuItemController(IMenuItemsRepository menuItemsRepository)
        {
            _menuItemsRepository = menuItemsRepository;
        }

        public IActionResult Index()
        {
            try
            {
                List<MenuItem> menuItems = _menuItemsRepository.GetAllMenuItems();
                return View(menuItems);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load menu items: " + ex.Message;
                return View(new List<MenuItem>());
            }
        }
    }
}