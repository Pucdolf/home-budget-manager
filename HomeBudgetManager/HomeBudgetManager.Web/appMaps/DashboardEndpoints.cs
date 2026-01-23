using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore;
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
                string adminBtnHtml = "";
                
                if (user.Role == SystemRole.SystemAdmin)
                {
                    adminBtnHtml = "<button class=\"sidebar-link\" onclick=\"window.location.href='/adminConsole'\">Ustawienia Admina</button>";
                }
                // 3. Podmieniamy nasz placeholder {username} na prawdziwą nazwę
                html =  html.Replace("{username}", username)
                            .Replace("{balance}", balance.ToString("N2"))
                            .Replace("{admin_panel_button}", adminBtnHtml);
                return Results.Content(html, "text/html");
            });

            // NOWY ENDPOINT DLA WYKRESÓW NA PULPICIE
            app.MapGet("/dashboard/charts", (HttpContext context, ChartService chartService) =>
            {
                if (!context.Request.Cookies.TryGetValue("user_id", out var userIdString) || 
                    !int.TryParse(userIdString, out int userId))
                {
                     return Results.Content(""); // Pusty content jeśli brak usera
                }

                var chartsHtml = chartService.GenerateDashboardChartsHtml(userId);
                return Results.Content(chartsHtml, "text/html");
            });
        }
    }
}