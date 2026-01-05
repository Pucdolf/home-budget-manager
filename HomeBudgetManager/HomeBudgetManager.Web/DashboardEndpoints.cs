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

            var user = await db.Users.FirstOrDefaultAsync(u => u.user_login == userLogin);

            if (user == null)
            {
                return Results.Content("<div class='error'>B³¹d: U¿ytkownik nieznaleziony.</div>", "text/html");
            }

            var transactions = await db.Transactions
                                .Where(t => t.DBUserId == user.user_id)
                                .OrderByDescending(t => t.transaction_date)
                                .ToListAsync();

            var sb = new System.Text.StringBuilder();

            foreach (var t in transactions)
            {
                string date = t.transaction_date.ToString("dd.MM.yyyy");
                string amount = t.transaction_value.ToString("C2", new System.Globalization.CultureInfo("pl-PL"));
                string colorClass = t.transaction_value < 0 ? "amount-expense" : "amount-income";

                sb.Append($"""

                    <li class="transaction-item">
                        <div class="transaction-amount {colorClass}">
                            {amount}
                        </div>
                        <div class="transaction-details">
                            <span class="category-badge">{t.transaction_category}</span>
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

            var user = await db.Users.FirstOrDefaultAsync(u => u.user_login == userLogin);

            if (user == null)
            {
                return Results.Content("<div class='error'>B³¹d: U¿ytkownik nieznaleziony.</div>", "text/html");
            }

            var form = context.Request.Form;
            var amount = decimal.Parse(form["amount"]);
            var description = form["description"].ToString();
            var category = form["category"].ToString();

            var transaction = new DBTransaction
            {
                transaction_category = category,
                transaction_date = DateTime.Now,
                transaction_description = description,
                transaction_is_repetable = false,
                transaction_value = amount,
                DBUserId = user.user_id,
                DBHouseId = user.user_house_id
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
                return Results.Content($"<div class='error'>B³¹d serwera: nie uda³o siê zapisaæ transakcji.</div>", "text/html");
            }
        });

        // DELETE - usuwanie transakcji
        group.MapDelete("/transactions", async (int id, HttpContext context, AppDbContext db) => {

            var userLogin = context.Request.Cookies["logged_user"];

            var user = await db.Users.FirstOrDefaultAsync(u => u.user_login == userLogin);
            if (user == null)
            {
                return Results.Content("<div class='error'>B³¹d: U¿ytkownik nieznaleziony.</div>", "text/html");
            }

            var transaction = await db.Transactions
                        .FirstOrDefaultAsync(t => t.transaction_id == id && t.DBUserId == user.user_id);

            if (transaction == null)
            {
                return Results.Content("<div class='error'>B³¹d: Transakcja nieznaleziona.</div>", "text/html");
            }

            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();

            return Results.Content("<div class='success'>Transakcja usuniêta</div>", "text/html");
        });

        // PUT - edycja transakcji
        group.MapPut("/transactions", async (int id, HttpContext context, AppDbContext db) => {

            var userLogin = context.Request.Cookies["logged_user"];

            var user = await db.Users.FirstOrDefaultAsync(u => u.user_login == userLogin);
            if (user == null)
            {
                return Results.Content("<div class='error'>B³¹d: U¿ytkownik nieznaleziony.</div>", "text/html");
            }

            var transaction = await db.Transactions
                            .FirstOrDefaultAsync(t => t.transaction_id == id && t.DBUserId==user.user_id);

            var form = context.Request.Form;
            transaction.transaction_value = decimal.Parse(form["amount"]);
            transaction.transaction_description = form["description"].ToString();
            transaction.transaction_category = form["category"].ToString();

            await db.SaveChangesAsync();

            return Results.Content("<div class='success'>Transakcja zaktualizowana!</div>", "text/html");
        });


        return group;
    }
}