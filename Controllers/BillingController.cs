using ElectroBillingMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ElectroBillingMVC.Controllers
{
    public class BillingController : Controller
    {
        // Temporary in-memory list
        private static List<Bill> bills = new List<Bill>();

        // ==========================
        // Customer Billing
        // ==========================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Bill bill)
        {
            bill.BillId = bills.Count + 1;
            bill.BillDate = DateTime.Now;
            bill.RemainingAmount = bill.TotalAmount - bill.PaidAmount;
            bill.Status = bill.RemainingAmount == 0 ? "Paid" : "Pending";

            bills.Add(bill);

            return RedirectToAction("History");
        }

        // ==========================
        // Customer History
        // ==========================
        public IActionResult History()
        {
            return View(bills);
        }

        // ==========================
        // Pending Payments
        // ==========================
        public IActionResult Pending()
        {
            var pending = bills.Where(b => b.Status == "Pending").ToList();
            return View(pending);
        }

        // ==========================
        // Paid Payments
        // ==========================
        public IActionResult Paid()
        {
            var paid = bills.Where(b => b.Status == "Paid").ToList();
            return View(paid);
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // No validation, always redirect to Dashboard
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Manager manager)
        {
            return RedirectToAction("Login", "Billing");
            
        }
        public IActionResult Start()
        {
            return View();
        }
        public IActionResult PaymentHistory(string customerName)
        {
            return View();
        }


    }
}