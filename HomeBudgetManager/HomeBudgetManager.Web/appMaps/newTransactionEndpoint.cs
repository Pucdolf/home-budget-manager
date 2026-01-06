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

        }
    }
}
