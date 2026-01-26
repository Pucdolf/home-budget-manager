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
                
                // Get returnUrl from query or Referer
                var returnUrl = context.Request.Query["returnUrl"].ToString();
                if (string.IsNullOrEmpty(returnUrl)) 
                {
                    returnUrl = context.Request.Headers["Referer"].ToString();
                }

                // Security/Business logic
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    if (returnUrl.Contains("/calendar")) 
                    {
                        returnUrl = "/calendar";
                    }
                    else if (returnUrl.Contains("/dashboard") && !returnUrl.Contains("/dashboard-household")) // Exclude household endpoint if it mimics dashboard
                    {
                        returnUrl = "/dashboard";
                    }
                    // Blocked pages: Stay on New Transaction page
                    else if (returnUrl.Contains("/reports") || returnUrl.Contains("/charts") || returnUrl.Contains("/household")) 
                    {
                         returnUrl = "/new-transaction";
                    }
                    else 
                    {
                        // Default fallback for unknown sources or direct access
                        returnUrl = "/dashboard";
                    }
                }
                else 
                {
                    returnUrl = "/dashboard";
                }

                // Save to cookie for robust retrieval
                context.Response.Cookies.Append("transaction_return_url", returnUrl, new CookieOptions { Expires = DateTime.Now.AddMinutes(30), Path = "/" });

                // load html with utf-8
                var filePath = Path.Combine(env.WebRootPath, "newTransaction.html");
                var html = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

                html = html.Replace("{username}", user.Login);
                // Inject returnUrl into a hidden field placeholder or JS variable if exists, 
                // but since I'll edit HTML I can add a placeholder there.
                // For now, I'll assume I will add {returnUrl} placeholder in HTML.
                html = html.Replace("{returnUrl}", returnUrl);

                string adminBtnHtml = "";

                if (user.Role == SystemRole.SystemAdmin)
                {
                    adminBtnHtml = "<button class=\"sidebar-link\" onclick=\"window.location.href='/adminConsole'\"><i class=\"fas fa-fw fa-cogs\"></i> &nbsp; Ustawienia Admina</button>";
                }
                html = html.Replace("{admin_panel_button}", adminBtnHtml);
                return Results.Content(html, "text/html; charset=utf-8");
            });

            // POST - add new transaction

            app.MapPost("/new-transaction/add", async (HttpContext context, AppDbContext db, TransactionService tranService) =>
            {

                var userLogin = context.Request.Cookies["logged_user"];
                var userId = int.Parse(context.Request.Cookies["user_id"]);
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return Results.Content("<div class='error'>Błąd: Użytkownik nieznaleziony.</div>", "text/html");
                }

                var form = context.Request.Form;

                // 0 - expense 1 - income
                var transactionType = int.Parse(form["transactionType"]);
                TransactionType type = (transactionType == 0) ? TransactionType.expense : TransactionType.income;

                var amountString = form["amount"].ToString();
                if (!decimal.TryParse(amountString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal amount))
                {
                    return Results.Content("<div class='error'>Błędny format kwoty! Użyj kropki np. 10.50</div>", "text/html");
                }
                amount = (type == 0) ? -amount : amount;

                var title = form["title"].ToString();
                if (string.IsNullOrWhiteSpace(title))
                {
                    return Results.Content("<div class='error'>Tytuł jest wymagany!</div>", "text/html");
                }
                if (title.Length > 20)
                {
                    return Results.Content("<div class='error'>Tytuł za długi (max 20 znaków)!</div>", "text/html");
                }

                var description = form["description"].ToString(); // Optional
                var category = int.Parse(form["categoryId"]);

                // Data
                string dateStr = form["transactionDate"].ToString();
                string timeStr = form["transactionTime"].ToString();

                bool isRecurring = form.ContainsKey("isRecurring");
                int? frequencyUnit = null;
                int? transactionInterval = null;

                if (isRecurring)
                {
                    var recurringType = form["recurringType"].ToString();
                    switch (recurringType)
                    {
                        case "daily":
                            frequencyUnit = 0; // Days
                            transactionInterval = 1;
                            break;
                        case "weekly":
                            frequencyUnit = 1; // Weeks
                            transactionInterval = 1;
                            break;
                        case "monthly":
                            frequencyUnit = 2; // Months
                            transactionInterval = 1;
                            break;
                        case "quarterly":
                            frequencyUnit = 2; // Months
                            transactionInterval = 3;
                            break;
                        case "yearly":
                            frequencyUnit = 3; // Years
                            transactionInterval = 1;
                            break;
                    }
                }

                DateTime finalDate;

                if (DateTime.TryParse($"{dateStr} {timeStr}", out DateTime parsedDate))
                {
                    finalDate = parsedDate;
                }
                else
                {
                    finalDate = DateTime.Now; // Fallback
                }

                try
                {
                    tranService.addTransaction(user.Id, category, amount, type, finalDate, isRecurring, transactionInterval, title, description, user.HouseId, frequencyUnit);
                    return Results.Content("<div class='success'>transakcja dodana</div>", "text/html");
                }
                catch (Exception ex)
                {
                    return Results.Content(ex.Message, "text/html");
                }
            });
        }
    }
}