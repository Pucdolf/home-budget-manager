using HomeBudgetManager.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace HomeBudgetManager.Web.appMaps
{
    public class DashboardEndpoints : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {

            // GET - Zaladuj dashboard

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

                var balance = await db.Transactions
                                    .Where(t => t.UserId == user.Id)
                                    .SumAsync(t => t.Value);

                // 1. Ścieżka do pliku HTML
                var filePath = Path.Combine(env.WebRootPath, "dashboard.html");

                // 2. Wczytujemy treść pliku do zmiennej
                // W prawdziwej produkcji warto by to cache'ować, ale dla prostego appa jest ok
                var html = File.ReadAllText(filePath);

                // 3. Podmieniamy nasz placeholder {username} na prawdziwą nazwę
                html = html.Replace("{username}", username);
                html = html.Replace("{balance}", balance.ToString("N2"));

                return Results.Content(html, "text/html");
            });
        }
    }
}
