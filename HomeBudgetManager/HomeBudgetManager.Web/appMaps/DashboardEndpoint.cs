using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Web.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace HomeBudgetManager.Web.appMaps
{
    public class DashboardEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/dashboard", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
            {
                // Sprawdzamy autoryzację
                if (!context.Request.Cookies.ContainsKey("logged_user"))
                {
                    return Results.Redirect("/");
                }

                var username = context.Request.Cookies["logged_user"];

                var user = await db.Users.FirstOrDefaultAsync(u => u.user_login == username);

                if (user == null)
                {
                    return Results.Redirect("/");
                }

                // Pobierz ostatnie transakcje. W take() decydujemy ile transakcji wyświetlić
                var transactions = await db.Transactions
                    .Where(t => t.DBUserId == user.user_id)
                    .OrderByDescending(t => t.transaction_date)
                    .Take(10)
                    .ToListAsync();

                var balance = await db.Transactions
                                    .Where(t => t.DBUserId == user.user_id)
                                    .SumAsync(t => t.transaction_value);

                // Generuj HTML dla transakcji
                var transactionsHtml = string.Join("", transactions.Select(t =>
                $@"
            <li class='transaction-item'>
                <div class='transaction-main'>
                    <span class='transaction-amount'>{(t.transaction_value >= 0 ? "+ " : "- ")}{Math.Abs(t.transaction_value):N2} zł</span>
                    <span class='transaction-category'>{t.transaction_category}</span>
                    <span class='transaction-date'>{t.transaction_date:yyyy-MM-dd}</span>
                </div>
                <div class='transaction-actions'>
                    <button class='btn-secondary' onclick='editTransaction({t.transaction_id})'>Edytuj</button>
                    <button class='btn-danger' hx-delete='/dashboard/transactions/{t.transaction_id}' hx-confirm='Czy na pewno chcesz usunąć tę transakcję?'>Usuń</button>
                </div>
            </li>
        "));

                // 1. Ścieżka do pliku HTML
                var filePath = Path.Combine(env.WebRootPath, "dashboard.html");

                // 2. Wczytujemy treść pliku do zmiennej
                // W prawdziwej produkcji warto by to cache'ować, ale dla prostego appa jest ok
                var html = File.ReadAllText(filePath);

                // 3. Podmieniamy nasz placeholder {username} na prawdziwą nazwę
                html = html.Replace("{username}", username);
                html = html.Replace("{balance}", balance.ToString("N2"));
                html = html.Replace("{transactions}", transactionsHtml);

                return Results.Content(html, "text/html");
            });


        }
    }
}
