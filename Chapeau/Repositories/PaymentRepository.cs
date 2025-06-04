using Chapeau.Models;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using Chapeau.ViewModels;

namespace Chapeau.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly string? _connectionString;

        public PaymentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }

        public List<Payment> GetPaymentSummaryForTable(int orderId)
        {
            List<Payment> paymentItemModels = new List<Payment>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT mi.name, mi.price, oi.count, mi.isAlcoholic, t.table_nr " +
                    "FROM [order] o " +
                    "JOIN [table] t ON o.table_id = t.table_id " +
                    "JOIN order_item oi ON oi.order_id = o.order_id " +
                    "JOIN menu_item mi ON oi.menu_item_id = mi.menu_item_id " +
                    "WHERE o.order_id = @OrderId;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Implementation details
                }
                return paymentItemModels;
            }
        }

        public void SavePayment(Payment payment, int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO payment (order_id, total_amount, tip, vat_value, payment_method, feedback)
                    VALUES (@OrderId, @TotalAmount, @TipAmount, @VatValue, @PaymentMethod, @Feedback)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Parameters.AddWithValue("@TotalAmount", payment.TotalAmount);
                command.Parameters.AddWithValue("@TipAmount", payment.Tip);
                command.Parameters.AddWithValue("@VatValue", payment.VatValue);
                command.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod.ToString());
                command.Parameters.AddWithValue("@Feedback", payment.FeedBack ?? (object)DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
} 