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
        }
    }
}
