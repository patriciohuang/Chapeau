using Microsoft.AspNetCore.Mvc;
using Chapeau.Services;
using Chapeau.Models;

namespace Chapeau.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        // GET: Menu
        [HttpGet]
        public IActionResult Index(string? id)
        {
            try
            {
                MenuCard? menuCard = null;
                if (id != null)
                {
                    menuCard = (MenuCard)Enum.Parse(typeof(MenuCard), id);
                }

                List<MenuItem> menuCourses = _menuService.GetAllCourses(menuCard);

                return View(menuCourses);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return RedirectToAction("Index", "Home");
            }
        }
    }
}
