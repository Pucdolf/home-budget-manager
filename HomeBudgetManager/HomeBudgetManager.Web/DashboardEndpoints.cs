using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace HomeBudgetManager.Web.Endpoints;

public static class DashboardEndpoints
{
    public static RouteGroupBuilder MapDashboardEndpoints(this RouteGroupBuilder group)
    {

        // GET - lista wszystkich transakcji
        group.MapGet("/transactions", async (HttpContext context, AppDbContext db) => {

            var userLogin = context.Request.Cookies["logged_user"];

            Console.WriteLine($"DEBUG: Cookie logged_user = '{userLogin}'");

            var user = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin);

            if (user == null)
            {
                return Results.Content("<div class='error'>Błąd: Użytkownik nieznaleziony.</div>", "text/html");
            }

            var transactions = await db.Transactions
                                .Where(t => t.UserId == user.Id)
                                .OrderByDescending(t => t.Date)
                                .ToListAsync();

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

        // POST - dodawanie nowej transakcji
        group.MapPost("/transactions", async (HttpContext context, AppDbContext db) => {
            
            var userLogin = context.Request.Cookies["logged_user"];

            Console.WriteLine($"DEBUG: Cookie logged_user = '{userLogin}'");

            var user = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin);

            if (user == null)
            {
                return Results.Content("<div class='error'>Błąd: Użytkownik nieznaleziony.</div>", "text/html");
            }

            var form = context.Request.Form;
            var amount = decimal.Parse(form["amount"]);
            var description = form["description"].ToString();
            var category = form["category"].ToString();

            var transaction = new DBTransaction
            {
                // TODO: FIX
                Category = null, //category,
                CategoryId = 0,
                Date = DateTime.Now,
                Description = description,
                IsRepeatable = false,
                Value = amount,
                UserId = user.Id,
                HouseId = user.HouseId
            };


            try
            {
                db.Transactions.Add(transaction);
                await db.SaveChangesAsync();
                return Results.Content("<div class='success'>transakcja dodana</div>", "text/html");

            } catch (Exception ex)
            {
                Console.WriteLine($"B³¹d zapisu: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
                }
                return Results.Content($"<div class='error'>Błąd serwera: nie udało się zapisać transakcji.</div>", "text/html");
            }
        });

        // DELETE - usuwanie transakcji
        group.MapDelete("/transactions", async (int id, HttpContext context, AppDbContext db) => {

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

            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();

            return Results.Content("<div class='success'>Transakcja usunięta</div>", "text/html");
        });

        // PUT - edycja transakcji
        group.MapPut("/transactions", async (int id, HttpContext context, AppDbContext db) => {

            var userLogin = context.Request.Cookies["logged_user"];

            var user = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin);
            if (user == null)
            {
                return Results.Content("<div class='error'>Błąd: Użytkownik nieznaleziony.</div>", "text/html");
            }

            var transaction = await db.Transactions
                            .FirstOrDefaultAsync(t => t.Id == id && t.UserId==user.Id);

            var form = context.Request.Form;
            transaction.Value = decimal.Parse(form["amount"]);
            transaction.Description = form["description"].ToString();
            // TODO: FIX
            //transaction.Category = form["category"].ToString();

            await db.SaveChangesAsync();

            return Results.Content("<div class='success'>Transakcja zaktualizowana!</div>", "text/html");
        });


        return group;
    }
}