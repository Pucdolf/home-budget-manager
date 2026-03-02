using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace HomeBudgetManager.Web.appMaps
{
    public class JoinHouseholdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/joinHousehold.html", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
            {
                if (!context.Request.Cookies.ContainsKey("logged_user"))
                    return Results.Redirect("/");
                var userId = int.Parse(context.Request.Cookies["user_id"]);
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return Results.Content("brak uzytkownika o takiej nazwie");
                }

                var username = context.Request.Cookies["logged_user"];
                if (user == null) return Results.Redirect("/");

                var filePath = Path.Combine(env.WebRootPath, "joinHousehold.html");
                if (!File.Exists(filePath)) return Results.NotFound();

                var html = await File.ReadAllTextAsync(filePath, Encoding.UTF8);

                string adminBtnHtml = "";
                if (user.Role == SystemRole.SystemAdmin)
                {
                    adminBtnHtml = "<button class=\"sidebar-link\" onclick=\"window.location.href='/adminConsole'\"><i class=\"fas fa-fw fa-cogs\"></i> &nbsp; Ustawienia Admina</button>";
                }

                html = html.Replace("{username}", username)
                           .Replace("{admin_panel_button}", adminBtnHtml);

                return Results.Content(html, "text/html; charset=utf-8");
            });

            app.MapPost("/join-household", async (HttpContext context, AppDbContext db) =>
            {
                var code = context.Request.Form["code"].ToString().ToUpper();
                var login = context.Request.Cookies["logged_user"];

                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == login);
                if (user == null || user.HouseId != null)
                {
                    return Results.Content("<div class='error'>Nie możesz dołączyć do nowego domostwa.</div>", "text/html");
                }

                var house = await db.Houses.FirstOrDefaultAsync(h => h.JoinCode == code);
                if (house == null)
                {
                    return Results.Content("<div class='error'>Nie znaleziono domostwa o takim kodzie.</div>", "text/html");
                }
                string adminBtnHtml = "";
                
                if (user.Role == SystemRole.SystemAdmin)
                {
                    adminBtnHtml = "<button class=\"sidebar-link\" onclick=\"window.location.href='/adminConsole'\"><i class=\"fas fa-fw fa-cogs\"></i> &nbsp; Ustawienia Admina</button>";
                }
                user.HouseId = house.Id;
                
                if (user.Role != SystemRole.SystemAdmin)
                {
                    user.Role = SystemRole.HouseholdMember;
                }

                await db.SaveChangesAsync();

                return Results.Content("<div class='success'>Dołączono do domostwa!</div>", "text/html");
            });

        }
    }
}
