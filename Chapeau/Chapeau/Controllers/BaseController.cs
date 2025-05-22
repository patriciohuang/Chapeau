using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Chapeau.Models;
using Chapeau.Services;
using Chapeau.Models.Enums;

namespace Chapeau.Controllers
{
    public abstract class BaseController : Controller
    {
        protected Employee CurrentEmployee => HttpContext.Session.GetObject<Employee>("LoggedInEmployee");

        // This runs automatically for every action in controllers that inherit from BaseController
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Check if user is authenticated
            if (CurrentEmployee == null)
            {
                context.Result = RedirectToAction("Login", "Auth");
                return;
            }
        }
    }
}