using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
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

                // load user
                var userId = int.Parse(context.Request.Cookies["user_id"]);
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                var username = context.Request.Cookies["logged_user"];
                if (user == null)
                {
                    return Results.Redirect("/");
                }

                var now = DateTime.Now;
                
                // set start to month before
                var startDate = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
                var endDate = now.ToString("yyyy-MM-dd");


                // load html
                var filePath = Path.Combine(env.WebRootPath, "reports.html");
                if (!File.Exists(filePath)) 
                    return Results.Content("Błąd: Brak pliku reports.html", "text/plain");

                var html = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
                string adminBtnHtml = "";
                
                if (user.Role == SystemRole.SystemAdmin)
                {
                    adminBtnHtml = "<button class=\"sidebar-link\" onclick=\"window.location.href='/adminConsole'\"><i class=\"fas fa-fw fa-cogs\"></i> &nbsp; Ustawienia Admina</button>";
                }
                // replace values in html
                html = html.Replace("{username}", username)
                           .Replace("{startDate}", startDate)
                           .Replace("{admin_panel_button}", adminBtnHtml)
                           .Replace("{endDate}", endDate)
                           .Replace("{household_display}", user.HouseId.HasValue ? "block" : "none");

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

                var scope = form["reportScope"].ToString();
                bool includeHousehold = scope == "household";

                var pdfBytes = reportService.GeneratePdfReport(userId, startDate, endDate, includeHousehold);
                var filename = $"Raport_HBM_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf";

                return Results.File(pdfBytes, "application/pdf", filename);
            });
        }
    }
}
