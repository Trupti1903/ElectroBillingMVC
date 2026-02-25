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

        // ==========================
        // CREATE BILL
        // ==========================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Bill bill)
        {
            // Remove validation for fields not coming from form
            ModelState.Remove("Status");
            ModelState.Remove("RemainingAmount");
            ModelState.Remove("BillDate");

            if (!ModelState.IsValid)
            {
                return View(bill);
            }

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            // Set calculated fields
            bill.RemainingAmount = bill.TotalAmount - bill.PaidAmount;
            bill.Status = bill.RemainingAmount == 0 ? "Paid" : "Pending";
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

        // ==========================
        // HISTORY
        // ==========================
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
                        Status = reader["Status"].ToString()
                    });
                }
            }

            return View(bills);
        }

        // ==========================
        // PENDING
        // ==========================
        public IActionResult Pending()
        {
            return GetBillsByStatus("Pending");
        }

        // ==========================
        // PAID
        // ==========================
        public IActionResult Paid()
        {
            return GetBillsByStatus("Paid");
        }

        private IActionResult GetBillsByStatus(string status)
        {
            List<Bill> bills = new List<Bill>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT * FROM Bills WHERE Status = @Status";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    bills.Add(new Bill
                    {
                        BillId = Convert.ToInt32(reader["BillId"]),
                        CustomerName = reader["CustomerName"].ToString(),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                        PaidAmount = Convert.ToDecimal(reader["PaidAmount"]),
                        RemainingAmount = Convert.ToDecimal(reader["RemainingAmount"]),
                        Status = reader["Status"].ToString()
                    });
                }
            }

            return View(bills);
        }

        // ==========================
        // LOGIN
        // ==========================
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

        // ==========================
        // REGISTER
        // ==========================
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

        // ==========================
        // DASHBOARD
        // ==========================
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("AdminEmail") == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // ==========================
        // LOGOUT
        // ==========================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}