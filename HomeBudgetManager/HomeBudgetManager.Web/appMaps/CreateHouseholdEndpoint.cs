using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace HomeBudgetManager.Web.appMaps
{
    public class CreateHouseholdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/createHousehold.html", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
            {
                if (!context.Request.Cookies.ContainsKey("logged_user"))
                    return Results.Redirect("/");

                var username = context.Request.Cookies["logged_user"].ToString();
                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == username);
                if (user == null) return Results.Redirect("/");

                var filePath = Path.Combine(env.WebRootPath, "createHousehold.html");
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

            app.MapPost("/create-household", async (HttpContext context, AppDbContext db) =>
            {
                var form = context.Request.Form;
                var name = form["name"];
                var description = form["description"];
                var userLogin = context.Request.Cookies["logged_user"];

                if (string.IsNullOrWhiteSpace(name))
                {
                    return Results.Content("<div class='error'>Błąd: nazwa grupy jest wymagana.</div>", "text/html");
                }

                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == userLogin);
                if (user == null)
                {
                    return Results.Content("<div class='error'>Błąd: użytkownik niezalogowany.</div>", "text/html");
                }

                if (user.HouseId != null)
                {
                    return Results.Content("<div class='error'>Błąd: użytkownik należy już do domostwa.</div>", "text/html");
                }

                // create household
                var house = new DBHouse
                {
                    Name = name,
                    Admin = user,
                    Description = description,
                    AdminId = user.Id,
                    JoinCode = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper() // ex. "A1B2C3"
                };
                db.Houses.Add(house);
                await db.SaveChangesAsync();

                // set user as household admin
                user.HouseId = house.Id;
                
                if (user.Role != SystemRole.SystemAdmin)
                {
                    user.Role = SystemRole.HouseholdAdmin;
                }

                await db.SaveChangesAsync();

                return Results.Content("<div class='success'>Domostwo utworzone!</div>", "text/html");
            });

        }
    }
}
