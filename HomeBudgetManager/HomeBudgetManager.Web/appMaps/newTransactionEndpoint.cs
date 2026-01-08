using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace HomeBudgetManager.Web.appMaps
{
    public class newTransactionEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/new-transaction", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
            {
                if (!context.Request.Cookies.ContainsKey("logged_user"))
                {
                    return Results.Redirect("/");
                }

                var userId = int.Parse(context.Request.Cookies["user_id"]);
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return Results.Content("brak uzytkownika o takiej nazwie");
                }

                var username = context.Request.Cookies["logged_user"];

                // Wczytaj plik HTML z kodowaniem UTF-8
                var filePath = Path.Combine(env.WebRootPath, "newTransaction.html");
                var html = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

                html = html.Replace("{username}", user.Login);

                return Results.Content(html, "text/html; charset=utf-8");
            });

            // POST - dodawanie nowej transakcji

            app.MapPost("/new-transaction/add", async (HttpContext context, AppDbContext db, TransactionService tranService) => {

                var userLogin = context.Request.Cookies["logged_user"];
                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin);

                if (user == null)
                {
                    return Results.Content("<div class='error'>B³¹d: U¿ytkownik nieznaleziony.</div>", "text/html");
                }

                var form = context.Request.Form;

                // 0 - expense 1 - income
                var transactionType = int.Parse(form["transactionType"]);
                TransactionType type = (transactionType == 0) ? TransactionType.expense : TransactionType.income;

                var amountString = form["amount"].ToString();
                if (!decimal.TryParse(amountString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal amount))
                {
                    return Results.Content("<div class='error'>B³êdny format kwoty! U¿yj kropki np. 10.50</div>", "text/html");
                }
                amount = (type == 0) ? -amount : amount;

                var description = form["description"].ToString();
                var category = int.Parse(form["categoryId"]);

                // Data
                string dateStr = form["transactionDate"].ToString();
                string timeStr = form["transactionTime"].ToString();

                bool isRecurring = form.ContainsKey("isRecurring");
                int? intervalDays = null;

                if (isRecurring)
                {
                    if (int.TryParse(form["recurringInterval"], out int days))
                    {
                        intervalDays = days;
                    }
                }

                DateTime finalDate;

                // U¿ywamy Parse, bo format z inputów HTML5 jest standardowy (ISO)
                if (DateTime.TryParse($"{dateStr} {timeStr}", out DateTime parsedDate))
                {
                    finalDate = parsedDate;
                }
                else
                {
                    finalDate = DateTime.Now; // Fallback w razie b³êdu
                }

                try
                {
                    tranService.addTransaction(user.Id, category, amount, type, finalDate, isRecurring, intervalDays, description, user.HouseId);
                    return Results.Content("<div class='success'>transakcja dodana</div>", "text/html");
                } catch (Exception ex)
                {
                    return Results.Content(ex.Message, "text/html");
                }
            });

        }
    }
}
