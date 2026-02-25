using ElectroBillingMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ElectroBillingMVC.Controllers
{
    public class BillingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public BillingController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Bill bill)
        {
            ModelState.Remove("Status");
            ModelState.Remove("RemainingAmount");
            ModelState.Remove("BillDate");

            if (!ModelState.IsValid)
            {
                return View(bill);
            }

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            // Calculate Remaining
            bill.RemainingAmount = bill.TotalAmount - bill.PaidAmount;

            if (bill.RemainingAmount <= 0)
            {
                bill.Status = "Paid";
                bill.RemainingAmount = 0;
                bill.PaidAmount = bill.TotalAmount;
            }
            else
            {
                bill.Status = "Pending";
            }

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

            return RedirectToAction("History");
        }

        public IActionResult History()
        {
            List<Bill> bills = new List<Bill>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Bills";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    bills.Add(new Bill
                    {
                        BillId = Convert.ToInt32(reader["BillId"]),
                        CustomerName = reader["CustomerName"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Address = reader["Address"].ToString(),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                        PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                        RemainingAmount = Convert.ToDecimal(reader["RemainingAmount"]),
                        BillDate = Convert.ToDateTime(reader["BillDate"]),
                        Status = reader["Status"].ToString()
                    });
                }
            }

            return View(bills);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteBill(int id)
        {
            var bill = _context.Bills.Find(id);

            if (bill != null)
            {
                _context.Bills.Remove(bill);
                _context.SaveChanges();
            }

            return RedirectToAction("Paid");
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReceivePayment(int id)
        {
            var bill = _context.Bills.Find(id);

            if (bill != null)
            {
                bill.PaidAmount = bill.TotalAmount;
                bill.RemainingAmount = 0;
                bill.Status = "Paid";

                _context.Bills.Update(bill);
                _context.SaveChanges();
            }

            return RedirectToAction("Pending");
        }


        public IActionResult Pending()
        {
            var pendingBills = _context.Bills
                .Where(b => b.Status == "Pending")
                .ToList();

            return View(pendingBills);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePayment(int id, decimal amount)
        {
            var bill = _context.Bills.Find(id);

            if (bill != null && amount > 0)
            {
                if (amount > bill.RemainingAmount)
                    amount = bill.RemainingAmount;

                bill.PaidAmount += amount;
                bill.RemainingAmount -= amount;

                if (bill.RemainingAmount <= 0)
                {
                    bill.RemainingAmount = 0;
                    bill.Status = "Paid";
                }
                else
                {
                    bill.Status = "Pending";
                }

                _context.Bills.Update(bill);
                _context.SaveChanges();
            }

            return RedirectToAction("Pending");
        }

        public IActionResult Paid()
        {
            var paidBills = _context.Bills
                .Where(b => b.Status == "Paid")
                .ToList();

            return View(paidBills);
        }

        public IActionResult PaymentHistory(string customerName)
        {
            List<Bill> bills = new List<Bill>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Bills WHERE CustomerName = @CustomerName";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@CustomerName", customerName);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    bills.Add(new Bill
                    {
                        BillId = Convert.ToInt32(reader["BillId"]),
                        CustomerName = reader["CustomerName"].ToString(),
                        Phone = reader["Phone"].ToString(),
                        Address = reader["Address"].ToString(),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                        PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                        RemainingAmount = Convert.ToDecimal(reader["RemainingAmount"]),
                        Status = reader["Status"].ToString(),
                        BillDate = Convert.ToDateTime(reader["BillDate"])
                    });
                }
            }

            return View(bills);
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Managers
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid Email or Password";
                return View();
            }

            HttpContext.Session.SetString("AdminEmail", user.Email);
            return RedirectToAction("Dashboard");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Manager manager)
        {
            _context.Managers.Add(manager);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}