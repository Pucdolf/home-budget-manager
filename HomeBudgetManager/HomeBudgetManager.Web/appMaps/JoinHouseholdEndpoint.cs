using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;

namespace HomeBudgetManager.Web.appMaps
{
    public class JoinHouseholdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
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

                user.HouseId = house.Id;
                user.Role = SystemRole.HouseholdMember;

                await db.SaveChangesAsync();

                return Results.Content("<div class='success'>Dołączono do domostwa!</div>", "text/html");
            });

        }
    }
}
