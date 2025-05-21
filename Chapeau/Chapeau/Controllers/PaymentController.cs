using Chapeau.Repositories;
using Chapeau.Models;
using Chapeau.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Chapeau.Controllers
{
    public class PaymentController : Controller
    {
       private readonly IPaymentRepository _paymentRepository;
        public PaymentController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }
        //payment information for a specific order
        public IActionResult Index(int orderId)
        {
            //gets a list of individual ordered items for a specific orderId
            List<PaymentItemModel> paymentItems = _paymentRepository.GetPaymentSummaryForTable(orderId);
            //wraps the list of PaymentItemModel objects inside a PaymentViewModel
            PaymentViewModel paymentViewModel = new PaymentViewModel(paymentItems);
            ///passes the PaymentViewModel to the Razor view for rendering
            return View(paymentViewModel);
        }
        
    }

}
