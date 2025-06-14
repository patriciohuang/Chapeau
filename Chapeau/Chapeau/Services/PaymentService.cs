using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Chapeau.Models;
using Chapeau.Models.Enums;
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

        public PaymentDetailsViewModel GetPaymentDetails(int orderId)
        {
            try
            {
                // Get the base order details
                var viewModel = GetOrderForPayment(orderId);

                // Get the existing payment for this order if any
                var payments = _paymentRepository.GetPaymentSummaryForTable(orderId);
                var lastPayment = payments.LastOrDefault();

                if (lastPayment != null)
                {
                    viewModel.TipAmount = lastPayment.Tip;
                    viewModel.Feedback = lastPayment.FeedBack;
                }

                return viewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PaymentDetailsViewModel GetOrderForPayment(int orderId)
        {
            try
            {
                Order order = _orderRepository.GetOrderById(orderId);
                
                // Group items by name and VAT rate
                var groupedItems = order.OrderItems
                    .GroupBy(item => new { item.MenuItem.Name, item.MenuItem.IsAlcoholic })
                    .Select(group => new PaymentDetailsViewModel.OrderItemViewModel
                    {
                        Name = group.Key.Name,
                        Price = group.First().MenuItem.Price,
                        Quantity = group.Sum(item => item.Count),
                        VATRate = group.Key.IsAlcoholic ? 21 : 9,
                        Comment = group.First().Comment ?? string.Empty
                    })
                    .ToList();

                var viewModel = new PaymentDetailsViewModel
                {
                    TableNr = order.Table.TableNr,
                    Items = groupedItems,
                    TotalLowVAT = order.TotalLowVAT,
                    TotalHighVAT = order.TotalHighVAT,
                    TipAmount = 0
                };

                viewModel.CalculateTotals();
                return viewModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ProcessPayment(PaymentProcessViewModel model)
        {
            try
            {
                // Get the order
                var order = _orderRepository.GetOrderById(model.OrderId);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }

                // Create payment record
                var payment = new Payment(
                    totalAmount: order.TotalCost + order.TotalVAT + model.TipAmount,
                    tip: model.TipAmount,
                    vatValue: (int)((order.TotalVAT / order.TotalCost) * 100), // Calculate VAT percentage
                    paymentMethod: model.PaymentMethod,
                    feedBack: model.Feedback ?? string.Empty // Handle null feedback
                );

                // Save payment
                _paymentRepository.SavePayment(payment, model.OrderId);

                // Update order status
                order.Status = Status.Completed;
                _orderRepository.UpdateOrderStatus(order.OrderId, order.Status);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public TipCalculationResult CalculateTip(int orderId, decimal value, bool isPercentage)
        {
            try
            {
                var order = _orderRepository.GetOrderById(orderId);
                if (order == null)
                {
                    throw new Exception("Order not found");
                }

                decimal baseAmount = order.TotalCost + order.TotalVAT;
                decimal tipAmount;

                if (isPercentage)
                {
                    // Calculate tip based on percentage
                    tipAmount = Math.Round((baseAmount * value) / 100, 2);
                }
                else
                {
                    // Use direct amount
                    tipAmount = Math.Round(value, 2);
                }

                // Ensure tip is not negative
                tipAmount = Math.Max(0, tipAmount);

                return new TipCalculationResult
                {
                    TipAmount = tipAmount,
                    NewTotal = baseAmount + tipAmount
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class TipCalculationResult
    {
        public decimal TipAmount { get; set; }
        public decimal NewTotal { get; set; }
    }
}
