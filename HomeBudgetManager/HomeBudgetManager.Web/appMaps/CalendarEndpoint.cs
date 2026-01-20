using System.Text;
using Microsoft.EntityFrameworkCore;
using HomeBudgetManager.Core;

namespace HomeBudgetManager.Web.appMaps
{
    public class CalendarEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/calendar", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
            {
                if (!context.Request.Cookies.ContainsKey("logged_user"))
                    return Results.Redirect("/");

                var username = context.Request.Cookies["logged_user"].ToString();
                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == username);
                if (user == null)
                    return Results.Redirect("/");

                var filePath = Path.Combine(env.WebRootPath, "calendar", "calendar.html");
                var html = File.ReadAllText(filePath, Encoding.UTF8);

                html = html.Replace("{username}", username);

                return Results.Content(html, "text/html; charset=utf-8");
            });

            app.MapGet("/api/calendar-events", async (HttpContext context, AppDbContext db) =>
            {
                if (!context.Request.Cookies.ContainsKey("logged_user"))
                    return Results.Unauthorized();

                var username = context.Request.Cookies["logged_user"];
                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == username);
                if (user == null)
                    return Results.Unauthorized();

                var transactions = await db.Transactions
                    .Where(t => t.UserId == user.Id)
                    .Select(t => new
                    {
                        id = t.Id.ToString(),
                        title = t.Value.ToString("F2") + " - " + (string.IsNullOrEmpty(t.Description) ? "Transakcja" : t.Description), // Show amount + desc
                        startTime = t.Date.ToString("yyyy-MM-ddTHH:mm:ss"), // ISO 8601
                        endTime = t.Date.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss"), // Dummy end time
                        amount = t.Value,
                        description = t.Description,
                        color = t.Value < 0 ? "#e74a3b" : "#1cc88a", // Red for expense, Green for income
                        reminder = false
                    })
                    .ToListAsync();

                return Results.Json(transactions);
            });
        }
    }
}
