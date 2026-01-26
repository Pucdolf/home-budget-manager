using HomeBudgetManager.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
namespace HomeBudgetManager.Web.appMaps
{
    public class ChartsEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/charts", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
            {
                
                // Ścieżka do pliku HTML
                var filePath = Path.Combine(env.WebRootPath, "charts.html");
                var html = await File.ReadAllTextAsync(filePath);

                // Basic layout similar to dashboard
                // We'll use a simple form to pick dates, defaulting to current month
                var now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
                var endDate = now.ToString("yyyy-MM-dd");
                string adminBtnHtml = "";
                var userId = int.Parse(context.Request.Cookies["user_id"]);
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                var username = context.Request.Cookies["logged_user"];
                if (user == null)
                {
                    return Results.Redirect("/");
                }
                if (user.Role == SystemRole.SystemAdmin)
                {
                    adminBtnHtml = "<button class=\"sidebar-link\" onclick=\"window.location.href='/adminConsole'\"><i class=\"fas fa-fw fa-cogs\"></i> &nbsp; Ustawienia Admina</button>";
                }
                html = html.Replace("{{username}}", username)
                           .Replace("{{startDate}}", startDate)
                           .Replace("{admin_panel_button}", adminBtnHtml)
                           .Replace("{{endDate}}", endDate)
                           .Replace("{household_display}", user.HouseId.HasValue ? "block" : "none");
                
                return Results.Content(html, "text/html");
            });

            app.MapPost("/charts/generate", (HttpContext context, ChartService chartService) =>
            {
                if (!context.Request.Cookies.TryGetValue("user_id", out var userIdString) || 
                    !int.TryParse(userIdString, out int userId))
                {
                    // Redirect to login or show error
                     return Results.Content("<div class='error'>Sesja wygasła. Zaloguj się ponownie.</div>", "text/html");
                }

                var form = context.Request.Form;
                if (!DateTime.TryParse(form["startDate"], out DateTime startDate) ||
                    !DateTime.TryParse(form["endDate"], out DateTime endDate))
                {
                     return Results.Content("<div class='error'>Nieprawidłowy format daty.</div>", "text/html");
                }
                
                string scope = form["scope"];
                bool includeHousehold = (scope == "house");

                // End date should include the whole day
                endDate = endDate.Date.AddDays(1).AddTicks(-1);

                var chartsHtml = chartService.GenerateChartsHtml(userId, startDate, endDate, includeHousehold);
                return Results.Content(chartsHtml, "text/html");
            });
        }
    }
}
