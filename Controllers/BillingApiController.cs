using ElectroBillingMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ElectroBillingMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BillingApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ✅ GET ALL BILLS
        [HttpGet]
        public IActionResult GetBills()
        {
            List<object> bills = new List<object>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Bills";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    bills.Add(new
                    {
                        BillId = Convert.ToInt32(reader["BillId"]),
                        CustomerName = reader["CustomerName"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                        PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                        RemainingAmount = Convert.ToDecimal(reader["RemainingAmount"]),
                        Status = reader["Status"].ToString()
                    });
                }
            }

            return Ok(bills);
        }

        // ✅ ADD BILL
        [HttpPost]
        public IActionResult AddBill([FromBody] Bill bill)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            bill.RemainingAmount = bill.TotalAmount - bill.PaidAmount;
            bill.Status = bill.RemainingAmount <= 0 ? "Paid" : "Pending";
            bill.BillDate = DateTime.Now;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO Bills
                (CustomerName, Phone, Address, TotalAmount, PaidAmount, RemainingAmount, BillDate, Status)
                VALUES
                (@CustomerName, @Phone, @Address, @TotalAmount, @PaidAmount, @RemainingAmount, @BillDate, @Status)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@CustomerName", bill.CustomerName);
                cmd.Parameters.AddWithValue("@Phone", bill.Phone);
                cmd.Parameters.AddWithValue("@Address", bill.Address);
                cmd.Parameters.AddWithValue("@TotalAmount", bill.TotalAmount);
                cmd.Parameters.AddWithValue("@PaidAmount", bill.PaidAmount);
                cmd.Parameters.AddWithValue("@RemainingAmount", bill.RemainingAmount);
                cmd.Parameters.AddWithValue("@BillDate", bill.BillDate);
                cmd.Parameters.AddWithValue("@Status", bill.Status);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return Ok("Bill Added Successfully");
        }

        // ✅ DELETE BILL
        [HttpDelete("{id}")]
        public IActionResult DeleteBill(int id)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "DELETE FROM Bills WHERE BillId=@id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return Ok("Deleted Successfully");
        }

        // ✅ UPDATE PAYMENT
        [HttpPut("pay/{id}")]
        public IActionResult UpdatePayment(int id, [FromBody] decimal amount)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string selectQuery = "SELECT * FROM Bills WHERE BillId=@id";
                SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                selectCmd.Parameters.AddWithValue("@id", id);

                SqlDataReader reader = selectCmd.ExecuteReader();

                if (!reader.Read())
                    return NotFound();

                decimal remaining = Convert.ToDecimal(reader["RemainingAmount"]);
                decimal paid = Convert.ToDecimal(reader["PaidAmount"]);
                decimal total = Convert.ToDecimal(reader["TotalAmount"]);

                reader.Close();

                if (amount > remaining)
                    amount = remaining;

                paid += amount;
                remaining -= amount;

                string status = remaining <= 0 ? "Paid" : "Pending";

                string updateQuery = @"UPDATE Bills 
                                      SET PaidAmount=@paid, RemainingAmount=@remaining, Status=@status 
                                      WHERE BillId=@id";

                SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                updateCmd.Parameters.AddWithValue("@paid", paid);
                updateCmd.Parameters.AddWithValue("@remaining", remaining);
                updateCmd.Parameters.AddWithValue("@status", status);
                updateCmd.Parameters.AddWithValue("@id", id);

                updateCmd.ExecuteNonQuery();
            }

            return Ok("Payment Updated");
        }
    }
}