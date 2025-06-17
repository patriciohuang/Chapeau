using Chapeau.Models;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using Chapeau.ViewModels;
using static Chapeau.ViewModels.PaymentViewModel;
using Chapeau.Models.Enums;

namespace Chapeau.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        //fields and properties
        private readonly string? _connectionString;

        //constructors
        public PaymentRepository(IConfiguration configuration)
        {
            //get (database) connectionstring from appsettings
            _connectionString = configuration.GetConnectionString("ChapeauDatabase");
        }


        // method to retrieve a summary of the ordered items for a specific table, based on a given orderId
        public List<Payment> GetPaymentsForOrder(int orderId)
        {
            // creates the list of the ordered items that need to be paid
            List<Payment> paymentItemModels = new List<Payment>();
            //connecting to the database
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM [payment] WHERE order_id = @OrderId;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                command.Connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    paymentItemModels.Add(ReadPaymentItemModel(reader));
                }
                return paymentItemModels;
            }

        }
       
       private Payment ReadPaymentItemModel(SqlDataReader reader)
        {
            int orderId = (int)reader["order_id"];
            decimal total = (decimal)reader["total_amount"];
            decimal tip = (decimal)reader["tip"];
            decimal vat = (decimal)reader["vat_value"];
            string method = (string)reader["payment_method"];
            PaymentMethod paymentMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), method);
            string feedback = (string)reader["feedback"];
            int paymentId = (int)reader["payment_id"];

            return new Payment(orderId, total, tip, vat, paymentMethod, feedback, paymentId);
        }

        public void SavePayment(Payment payment)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    INSERT INTO payment (order_id, total_amount, tip, vat_value, payment_method, feedback)
                    VALUES (@OrderId, @TotalAmount, @TipAmount, @VatValue, @PaymentMethod, @Feedback)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", payment.OrderId);
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



