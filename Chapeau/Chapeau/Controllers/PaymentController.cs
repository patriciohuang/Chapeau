using Chapeau.Services;
using Chapeau.Models;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Chapeau.Models.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Text.Json;

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
                var viewModel = _paymentService.GetPaymentDetails(orderId);
                
                // Set the feedback from TempData if it exists
                if (TempData["Feedback"] is string feedback)
                {
                    viewModel.Feedback = feedback;
                }
                
                ViewBag.OrderId = orderId;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the error
                return RedirectToAction("Index", "Waiter");
            }
        }

        [HttpPost]
        public IActionResult ProcessPayment([FromBody] PaymentProcessViewModel model)
        {
            try
            {
                bool success = _paymentService.ProcessPayment(model);
                if (success)
                {
                    // Redirect to PaymentSuccess page
                    return Json(new { success = true, redirectUrl = Url.Action("PaymentSuccess", "Payment") });
                }
                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Payment processing error: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }

        public IActionResult PaymentSuccess()
        {
            return View();
        }

        public IActionResult SplitPay()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveFeedback(int orderId, string feedback)
        {
            try
            {
                var viewModel = _paymentService.GetPaymentDetails(orderId);
                viewModel.Feedback = feedback;
                
                // Store the feedback in TempData to persist across redirects
                TempData["Feedback"] = feedback;
                
                return RedirectToAction("Index", new { orderId = orderId });
            }
            catch (Exception ex)
            {
                // Log the error
                return RedirectToAction("Index", new { orderId = orderId, error = "Failed to save feedback" });
            }
        }

        public IActionResult SplitByDish(int orderId)
        {
            try
            {
                var viewModel = _paymentService.GetPaymentDetails(orderId);
                return View("SplitByDish", viewModel);
            }
            catch (Exception ex)
            {
                // Log the error
                return RedirectToAction("Index", new { orderId });
            }
        }

        public IActionResult SplitByAmount(int orderId)
        {
            try
            {
                var viewModel = _paymentService.GetPaymentDetails(orderId);
                return View("SplitByAmount", viewModel);
            }
            catch (Exception ex)
            {
                // Log the error
                return RedirectToAction("Index", new { orderId });
            }
        }

        [HttpPost]
        public IActionResult CalculateTip([FromBody] TipCalculationRequest request)
        {
            try
            {
                var result = _paymentService.CalculateTip(request.OrderId, request.Value, request.IsPercentage);
                return Json(new { 
                    success = true, 
                    tipAmount = result.TipAmount.ToString("0.00"),
                    newTotal = result.NewTotal,
                    formattedTip = $"€{result.TipAmount:0.00}",
                    formattedTotal = $"€{result.NewTotal:0.00}"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }

    public class TipCalculationRequest
    {
        public int OrderId { get; set; }
        public decimal Value { get; set; }
        public bool IsPercentage { get; set; }
    }
}



