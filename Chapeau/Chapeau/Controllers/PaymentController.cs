using Chapeau.Services;
using Chapeau.Models;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Models.Enums;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chapeau.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // This method runs before every action in this controller
        // It adds an additional role check specifically for Waiters
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // First run the base authentication check (from BaseController)
            base.OnActionExecuting(context);

            // If already redirecting due to no authentication, don't continue
            if (context.Result != null) return;

            // Check if user has Waiter role
            if (CurrentEmployee == null || !CurrentEmployee.Role.Equals(RoleNames.Waiter, StringComparison.OrdinalIgnoreCase))
            {
                // User doesn't have Waiter role - redirect to unauthorized page
                context.Result = RedirectToAction("Unauthorized", "Auth");
            }
        }

        public IActionResult Index(int orderId)
        {
            try
            {
                PaymentDetailsViewModel paymentDetails = _paymentService.GetOrderForPayment(orderId);
                return View(paymentDetails);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}



