## 🏠 HomeBudgetManager – Comprehensive Home Budget Management System

**HomeBudgetManager** is a modern web application designed for monitoring expenses and income, allowing users full control over their personal finances and shared household budgets. This project was developed as part of the *Computer Programming* course at the Silesian University of Technology.

---

### 🕹️ Preview / Gallery

<p align="center">
  <img src="https://github.com/user-attachments/assets/0bc44495-17dc-48f9-a6c2-dbb122f830be" width="700" alt="HomeBudgetManager Demo">
</p>

#### 📄 PDF Report Preview
<p align="center">
  <img src="https://github.com/user-attachments/assets/5ab63780-1fda-4dec-ba35-e5615c0523de" width="200" alt="Report Page 1">
  <img src="https://github.com/user-attachments/assets/33068a7f-91e1-4e95-a24f-b9ace0378e59" width="200" alt="Report Page 2">
  <img src="https://github.com/user-attachments/assets/925e9b39-064f-416f-a366-1fb94f6b8fd7" width="200" alt="Report Page 3">
  <img src="https://github.com/user-attachments/assets/938c3519-e4aa-498a-abd6-bfd332241d2f" width="200" alt="Report Page 4">
</p>

---

### 🏗️ Architecture and Technologies

The project is based on a layered architecture, ensuring separation of business logic from presentation:

*   **`HomeBudgetManager.Core`** – Class library containing domain logic, database entities, business services, and migrations. It uses **Entity Framework Core** with the **SQLite** provider.
*   **`HomeBudgetManager.Web`** – ASP.NET Core web application utilizing the **Minimal APIs** pattern. It is responsible for serving the frontend (HTML/JS) and handling API requests. It also includes a **Background Service** for automatic processing of recurring transactions.
*   **`HomeBudgetManager.Tests`** – Testing project using **xUnit**, used to verify the correctness of business logic (transactions, authorization, household management).

**Technologies Used:**
*   **Backend:** .NET 9.0, C#, ASP.NET Core
*   **Database:** SQLite, Entity Framework Core (EF Core)
*   **Security:** `PasswordHasher` (Microsoft.AspNetCore.Identity) for secure password hashing, Cookie-based authentication.
*   **Reporting:** QuestPDF (PDF document generation).
*   **Frontend:** Vanilla HTML5, CSS3, JavaScript (ES6+).
*   **Testing:** xUnit, Moq, FluentAssertions.

---

### 📂 Key Functionalities

*   **Transaction Management** – Full CRUD support for transactions, categorized into income and expenses.
*   **Automation (Recurring Transactions)** – The system has a built-in background process (**Worker Service**) that checks every hour and automatically generates scheduled transactions (daily, monthly, etc.).
*   **Interactive Calendar** – Visualization of financial history and forecasts.
*   **Advanced PDF Reports** – Generation of professional summaries with bar charts and detailed operation lists.
*   **Households** – Budget sharing with other users and joint tracking of household expenses.
*   **Security and Roles** – Role-based system (User, Admin), secure storage of credentials and sessions.
*   **Admin Console** – A panel for direct data management using SQL queries (restricted to system administrators).

---

### 🛠️ Requirements and Configuration

*   **.NET 9.0 SDK**
*   Operating System: Windows, Linux, or macOS.
*   Optional: EF Core command-line tools (`dotnet-ef`).

**Database Configuration:**
Connection parameters are located in the `HomeBudgetManager.Web/appsettings.json` file. By default, the application uses a local SQLite database file.

---

## 🚀 Compilation, Running, and Tests

### 1. Clone the repository
```bash
git clone https://github.com/Pucdolf/home-budget-manager.git
cd home-budget-manager
```

### 2. Database Preparation
If you want to update the database to the latest migration:
```bash
dotnet ef database update --project HomeBudgetManager.Core --startup-project HomeBudgetManager.Web
```

### 3. Running the application
```bash
dotnet run --project HomeBudgetManager.Web
```
The application will be available at `http://localhost:5000` (or as indicated in the console).

### 4. Running tests
To ensure everything is working correctly, run the unit test suite:
```bash
dotnet test
```

---

### 🎨 Resources and License
This project is developed for educational purposes as open-source. It utilizes libraries under Community/MIT licenses (including QuestPDF, SQLite).
