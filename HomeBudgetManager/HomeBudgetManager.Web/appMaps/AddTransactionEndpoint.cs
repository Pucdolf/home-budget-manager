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
    public class AddTransactionEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/add-transaction", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
            {
                if (!context.Request.Cookies.ContainsKey("logged_user"))
                {
                    return Results.Redirect("/");
                }

                var userId = int.Parse(context.Request.Cookies["user_id"]);
                var user = await db.Users.FirstOrDefaultAsync(u => u.user_id == userId);
                var username = context.Request.Cookies["logged_user"];

                // Wczytaj plik HTML z kodowaniem UTF-8
                var filePath = Path.Combine(env.WebRootPath, "addTransaction.html");
                var html = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

                html = html.Replace("{username}", user.user_login);

                return Results.Content(html, "text/html; charset=utf-8");
            });

        }
    }
}
