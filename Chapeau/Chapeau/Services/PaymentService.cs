using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Chapeau.Models;
using Chapeau.Repositories;
using Chapeau.ViewModels;

namespace Chapeau.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        public PaymentDetailsViewModel GetOrderForPayment(int orderId)
        {
            try
            {
                Order order = _orderRepository.GetOrder(orderId);
                
                // Group items by name and VAT rate
                var groupedItems = order.OrderItems
                    .GroupBy(item => new { item.MenuItem.Name, item.MenuItem.IsAlcoholic })
                    .Select(group => new PaymentDetailsViewModel.OrderItemViewModel
                    {
                        Name = group.Key.Name,
                        Price = group.First().MenuItem.Price,
                        Quantity = group.Sum(item => item.Count),
                        Amount = group.Sum(item => item.MenuItem.Price * item.Count),
                        VATRate = group.Key.IsAlcoholic ? "21%" : "9%"
                    })
                    .ToList();

                var viewModel = new PaymentDetailsViewModel
                {
                    Items = groupedItems
                };

                // Calculate totals
                viewModel.Subtotal = order.TotalCost;
                viewModel.TotalHighVAT = order.TotalHighVAT;
                viewModel.TotalLowVAT = order.TotalLowVAT;
                viewModel.TotalVAT = order.TotalVAT;
                viewModel.GrandTotal = order.TotalCost + order.TotalVAT;

                return viewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
