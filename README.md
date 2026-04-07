# ⚡ ElectroBilling – Electronic Shop Billing System (ASP.NET MVC)

## 📌 Project Overview
ElectroBilling is an ASP.NET MVC-based web application developed to manage billing operations for an electronic shop.  

The system helps in handling customers, products, billing, and payment tracking (paid/pending) efficiently with a structured and secure approach.

---

## 🚀 Features

### 🔐 Authentication
- Secure login system for Manager/Admin
- Session-based access control
- Unauthorized access restriction

### 📊 Dashboard
- Overview of total customers, products, and bills
- Quick navigation to all modules

### 👤 Customer Management
- Add new customers
- View customer details
- Maintain customer history

### 🧾 Billing System
- Generate bills for customers
- Add multiple products to a bill
- Automatic total calculation

### 💰 Payment Tracking
- Separate sections for:
  - ✅ Paid Payments
  - ⏳ Pending Payments
- Update payment status

---

## 🛠️ Technologies Used

- **Frontend:** HTML, CSS, Bootstrap  
- **Backend:** ASP.NET MVC (C#)  
- **Database:** SQL Server  
- **Framework:** .NET  

---

## 📂 Project Structure

```
ElectroBilling/
│
├── Controllers/
│   ├── AccountController.cs
│   ├── BillingController.cs
│   ├── CustomerController.cs
│   ├── ProductController.cs
│
├── Models/
│   ├── Customer.cs
│   ├── Product.cs
│   ├── Billing.cs
│
├── Views/
│   ├── Account/
│   ├── Billing/
│   ├── Customer/
│   ├── Product/
│   ├── Shared/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│
├── appsettings.json
├── Program.cs
└── README.md
```

---

## ⚙️ Installation & Setup

1. Clone the repository
```
git clone https://github.com/HimaniSolanki1812/ElectroBillingMVC.git.git
```

2. Open the project in Visual Studio

3. Configure database connection  
- Go to `appsettings.json`
  
---

4. Run the project  
- Press **F5** or click **Start**

---

## 🗄️ Database

- SQL Server is used for data storage
- Main tables:
  - Customers
  - Products
  - Bills / Orders
  - Payments

---

## 🎯 Use Case

This system is useful for:
- Electronic shop owners
- Small retail businesses
- Billing and payment tracking systems

---

## 🔒 Security Features

- Login authentication
- Session handling
- Route protection (prevent unauthorized access)
- Input validation

---


## 👩‍💻 Author

**Himani Solanki**  
**Apexa Pandit**
**Trupti Malmadi**

---

## ⭐ Support

If you like this project, give it a ⭐ on GitHub!
