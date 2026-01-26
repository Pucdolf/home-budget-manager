 using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace HomeBudgetManager.Web.appMaps;

public class TransactionsEndpoints : IEndpoint
{
    public void Map(IEndpointRouteBuilder app)
    {
        // GET - list of all transactions

        app.MapGet("/transactions", async (HttpContext context, AppDbContext db, TransactionService tranService) => {

            var userLogin = context.Request.Cookies["logged_user"];

            Console.WriteLine($"DEBUG: Cookie logged_user = '{userLogin}'");
            var userId = int.Parse(context.Request.Cookies["user_id"]);
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Results.Content("<div class='error'>Błąd: Użytkownik nieznaleziony.</div>", "text/html");
            }

            var transactions = tranService.AllUserTransactions(user.Id);

            var sb = new System.Text.StringBuilder();

            foreach (var t in transactions)
            {
                string date = t.Date.ToString("dd.MM.yyyy");
                string amount = t.Value.ToString("C2", new System.Globalization.CultureInfo("pl-PL"));
                string colorClass = t.Value < 0 ? "amount-expense" : "amount-income";

                sb.Append($"""

                    <li class="transaction-item">
                        <div class="transaction-amount {colorClass}">
                            {amount}
                        </div>
                        <div class="transaction-details">
                            <span class="category-badge">{t.Category}</span>
                            <span class="transaction-date">{date}</span>
                        </div>
                    </li>

                 """);
            }

            return Results.Content(sb.ToString(), "text/html");
        });

        app.MapGet("/transactions/listSome", async (HttpContext context, AppDbContext db, TransactionService tranService) =>
        {

            var userLogin = context.Request.Cookies["logged_user"];

            Console.WriteLine($"DEBUG: Cookie logged_user = '{userLogin}'");
            var userId = int.Parse(context.Request.Cookies["user_id"]);
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Results.Content("<div class='error'>Błąd: Użytkownik nieznaleziony.</div>", "text/html");
            }

            var transactions = tranService.SomeUserTransactions(user.Id, 5);

            return Results.Content(tranService.listTransactionsForDashboard(transactions).ToString(), "text/html");
        });

        // DELETE transaction
        app.MapDelete("/transactions", async (int id, HttpContext context, AppDbContext db) => {

            var userLogin = context.Request.Cookies["logged_user"];
            var userId = int.Parse(context.Request.Cookies["user_id"]);
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return Results.Content("<div class='error'>Błąd: Użytkownik nieznaleziony.</div>", "text/html");
            }

            var transaction = await db.Transactions
                        .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            if (transaction == null)
            {
                return Results.Content("<div class='error'>Błąd: Transakcja nieznaleziona.</div>", "text/html");
            }

            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();

            return Results.Content("<div class='success'>Transakcja usunięta</div>", "text/html");
        });

        // PUT - edit transaction
        app.MapPut("/transactions", async (int id, HttpContext context, AppDbContext db) => {

            var userLogin = context.Request.Cookies["logged_user"];

            var user = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin);
            if (user == null)
            {
                return Results.Content("<div class='error'>Błąd: Użytkownik nieznaleziony.</div>", "text/html");
            }

            var transaction = await db.Transactions
                            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);
            
            if (transaction == null)
            {
                 return Results.Content("<div class='error'>Błąd: Transakcja nieznaleziona.</div>", "text/html");
            }

            var form = context.Request.Form;
            
            // Transaction Type (0=Expense, 1=Income)
            int transType = -1;
            if (form.ContainsKey("transactionType"))
            {
                int.TryParse(form["transactionType"], out transType);
            }

            if (form.ContainsKey("amount"))
            {
                 // Handle decimal parsing with invariant culture (dot separator)
                 if (decimal.TryParse(form["amount"], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal val))
                 {
                     val = Math.Abs(val); // Start positive
                     if (transType == 0) // Expense
                         val = -val;
                     else if (transType == 1) // Income
                         val = val; // Positive
                     else 
                     {
                         // If type not sent, preserve existing sign? Or assume expense?
                         // Better: check existing sign if type not provided, but form should provide it.
                         // If we are editing, we probably sent the type.
                         // If type is not 0 or 1, maybe keep current sign logic? 
                         // Let's rely on the provided type or infer from current value if missing
                         if (transaction.Value < 0) val = -val;
                     }
                     
                     transaction.Value = val;
                     transaction.TransactionType = (val < 0) ? TransactionType.expense : TransactionType.income;
                 }
            }
            
            if (form.ContainsKey("description"))
                transaction.Description = form["description"].ToString();

            if (form.ContainsKey("title"))
            {
                var t = form["title"].ToString();
                if (!string.IsNullOrWhiteSpace(t) && t.Length <= 20)
                {
                    transaction.Title = t;
                }
            }

            // Date and Time merging
            string dateStr = form["transactionDate"];
            string timeStr = form["transactionTime"];
            
            if (!string.IsNullOrEmpty(dateStr))
            {
                // If time is missing, default to 00:00 or keep existing time?
                // The form provides both.
                string fullDateStr = dateStr;
                if (!string.IsNullOrEmpty(timeStr))
                {
                    fullDateStr += " " + timeStr;
                }
                
                if (DateTime.TryParse(fullDateStr, out DateTime newDate))
                {
                    transaction.Date = newDate;
                }
            } else if (form.ContainsKey("date") && DateTime.TryParse(form["date"], out DateTime legacyDate)) {
                 transaction.Date = legacyDate;
            }

            if (form.ContainsKey("categoryId") && int.TryParse(form["categoryId"], out int catId))
            {
                transaction.CategoryId = catId;
            }

            await db.SaveChangesAsync();

            return Results.Content("<div class='success'>Transakcja zaktualizowana!</div>", "text/html");
        });
    }
}