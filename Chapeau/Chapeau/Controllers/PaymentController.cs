using Chapeau.Repositories;
using Chapeau.Models;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Models.Enums;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chapeau.Controllers
{
    public class PaymentController : BaseController
    {
       private readonly IPaymentRepository _paymentRepository;
        public PaymentController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
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
        //payment information for a specific order
        public IActionResult Index(int orderId)
        {
            // Double-check the user has Waiter role access
            var accessResult = CheckAccess(UserRole.Waiter);
            if (accessResult != null) return accessResult;

            // Get payment items for the specified order
            List<PaymentItemModel> paymentItems = _paymentRepository.GetPaymentSummaryForTable(orderId);

            // Create view model with payment items
            PaymentViewModel paymentViewModel = new PaymentViewModel(paymentItems);

            // Show payment summary view
            return View(paymentViewModel);
        }
        
    }

}
