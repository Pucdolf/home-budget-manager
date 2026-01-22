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

                List<int> userIds = new List<int>();
                if (user.HouseId.HasValue)
                {
                    userIds = await db.Users
                        .Where(u => u.HouseId == user.HouseId.Value)
                        .Select(u => u.Id)
                        .ToListAsync();
                }
                else
                {
                    userIds.Add(user.Id);
                }

                var transactions = await db.Transactions
                    .Include(t => t.User) // Include User to get Login
                    .Where(t => userIds.Contains(t.UserId))
                    .OrderBy(t => t.Date) // Sort by date/time
                    .Select(t => new
                    {
                        id = t.Id.ToString(),
                        title = $"{t.Value:F2} ({t.User.Login})", // Show amount + user
                        startTime = t.Date.ToString("yyyy-MM-ddTHH:mm:ss"),
                        endTime = t.Date.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ss"),
                        amount = t.Value,
                        description = t.Description ?? "",
                        categoryId = t.CategoryId,
                        color = t.Value < 0 ? "#e74a3b" : "#1cc88a",
                        reminder = false
                    })
                    .ToListAsync();

                return Results.Json(transactions);
            });
        }
    }
}
