using HomeBudgetManager.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using System.Text;

using System.Globalization;
namespace HomeBudgetManager.Web.appMaps
{
    public class ReportsEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/reports", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
            {
                // 1. Sprawdzenie ciastka
                if (!context.Request.Cookies.TryGetValue("logged_user", out var username) || string.IsNullOrEmpty(username))
                {
                    return Results.Redirect("/");
                }

                // 2. Pobranie usera z bazy (potrzebne do sprawdzenia roli Admina)
                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == username);
                if (user == null)
                {
                    return Results.Redirect("/");
                }

                // 3. Obliczenie domyślnych dat (pierwszy dzień miesiąca - dzisiaj)
                var now = DateTime.Now;
                
                // Ustawiamy start na miesiąc wstecz od dzisiaj (np. od 23.12 do 23.01)
                var startDate = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
                var endDate = now.ToString("yyyy-MM-dd");


                // 4. Wczytanie pliku HTML
                var filePath = Path.Combine(env.WebRootPath, "reports.html");
                if (!File.Exists(filePath)) 
                    return Results.Content("Błąd: Brak pliku reports.html", "text/plain");

                var html = await File.ReadAllTextAsync(filePath, Encoding.UTF8);

                // 6. Podmiana danych w HTML
                html = html.Replace("{username}", username)
                           .Replace("{startDate}", startDate)
                           .Replace("{endDate}", endDate);

                return Results.Content(html, "text/html; charset=utf-8");
            });


            app.MapPost("/reports/generate", (HttpContext context, ReportService reportService) =>
            {
                if (!context.Request.Cookies.TryGetValue("user_id", out var userIdString) || 
                    !int.TryParse(userIdString, out int userId))
                {
                    return Results.Redirect("/");
                }

                var form = context.Request.Form;
                if (!DateTime.TryParse(form["startDate"], out DateTime startDate) ||
                    !DateTime.TryParse(form["endDate"], out DateTime endDate))
                {
                     return Results.Content("Nieprawidłowa data");
                }
                
                // End of day
                endDate = endDate.Date.AddDays(1).AddTicks(-1);

                var pdfBytes = reportService.GeneratePdfReport(userId, startDate, endDate);
                var filename = $"Raport_HBM_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf";

                return Results.File(pdfBytes, "application/pdf", filename);
            });
        }
    }
}
