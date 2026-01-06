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

            // wyswietl 10 pierwszych transakcji

            app.MapGet("/dashboard", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
            {
                // Sprawdzamy autoryzację
                if (!context.Request.Cookies.ContainsKey("logged_user"))
                {
                    return Results.Redirect("/");
                }

                var username = context.Request.Cookies["logged_user"];

                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == username);

                if (user == null)
                {
                    return Results.Redirect("/");
                }

                // Pobierz ostatnie transakcje. W take() decydujemy ile transakcji wyświetlić
                var transactions = await db.Transactions
                    .Where(t => t.UserId == user.Id)
                    .OrderByDescending(t => t.Date)
                    .Take(10)
                    .ToListAsync();

                var balance = await db.Transactions
                                    .Where(t => t.UserId == user.Id)
                                    .SumAsync(t => t.Value);

                // Generuj HTML dla transakcji
                var transactionsHtml = string.Join("", transactions.Select(t =>
                $@"
            <li class='transaction-item'>
                <div class='transaction-main'>
                    <span class='transaction-amount'>{(t.Value >= 0 ? "+ " : "- ")}{Math.Abs(t.Value):N2} zł</span>
                    <span class='transaction-category'>{t.Category}</span>
                    <span class='transaction-date'>{t.Date:yyyy-MM-dd}</span>
                </div>
                <div class='transaction-actions'>
                    <button class='btn-secondary' onclick='editTransaction({t.Id})'>Edytuj</button>
                    <button class='btn-danger' hx-delete='/dashboard/transactions/{t.Id}' hx-confirm='Czy na pewno chcesz usunąć tę transakcję?'>Usuń</button>
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
