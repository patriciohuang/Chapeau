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
        //TODO nest both of these repositories into the payment service
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        
        public PaymentController(IPaymentRepository paymentRepository, IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
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
            //TODO: add a try catch block to handle exceptions and "make the code more robust"

            //How I (Jeroen) would implement it: After you retrieve the order, you can calculate the total price and VAT using a service method/the controller. Then put those three in a viewModel and return that to the view.


            // Double-check the user has Waiter role access
            var accessResult = CheckAccess(UserRole.Waiter);
            if (accessResult != null) return accessResult;


            Order order = _orderRepository.GetOrder(orderId);

            //TODO add a service that automatically calculates the total price/VAT of the order, you can also do this in the controller to make your life easier

            return View(order);




            //Commented this old code below because I don't want to remove other people's work

            /*
            // Get payment items for the specified order
            List<Payment> paymentItems = _paymentRepository.GetPaymentSummaryForTable(orderId);

            // Create view model with payment items
            PaymentViewModel paymentViewModel = new PaymentViewModel(paymentItems);

            // Show payment summary view
            return View(paymentViewModel);
            */
        }
        
    }

}
