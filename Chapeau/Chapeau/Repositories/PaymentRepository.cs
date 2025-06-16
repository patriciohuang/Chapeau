using Chapeau.Models;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using Chapeau.ViewModels;
using static Chapeau.ViewModels.PaymentViewModel;

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
        public List<Payment> GetPaymentSummaryForTable(int orderId)
        {
            // creates the list of the ordered items that need to be paid
            List<Payment> paymentItemModels = new List<Payment>();
            //connecting to the database
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // query to get the name of the dish, price, how many, whether it's alcohol or not for VAT and table number
                string query = "SELECT mi.name, mi.price, oi.count, mi.isAlcoholic, t.table_nr " +
                    "FROM [order] o " +
                    "JOIN [table] t ON o.table_id = t.table_id " +
                    "JOIN order_item oi ON oi.order_id = o.order_id " +
                    "JOIN menu_item mi ON oi.menu_item_id = mi.menu_item_id " +
                    
                    "WHERE o.order_id = @OrderId;";
                SqlCommand command = new SqlCommand(query, connection);
                //Adds a parameter to the SQL query.
                command.Parameters.AddWithValue("@OrderId", orderId);
                //Opens the connection to the database so that the command can be executed.
                command.Connection.Open();
                //Executes the SQL command and returns a SqlDataReader object.
                SqlDataReader reader = command.ExecuteReader();
                //Loops through each row in the result set.
                while (reader.Read())
                {
                    //please check this after
                    /*paymentItemModels.Add(ReadPaymentItemModel(reader));*/
                }
                //the list of PaymentItemModel objects is returned.
                return paymentItemModels;
            }

        }
        // takes the current row from a SQL result and turns it into a usable object
        //please check this after
        /*private Payment ReadPaymentItemModel(SqlDataReader reader)
        {
            string name = (string)reader["name"];
            decimal price = (decimal)reader["price"];
            int count = (int)reader["count"];
            bool isAlcoholic = (bool)reader["isAlcoholic"];
            int tableNr = (int)reader["table_nr"];

            return new Payment(name, price, count, isAlcoholic, tableNr);
        }*/

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



