using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;

namespace HomeBudgetManager.Web.appMaps
{
    public class CreateHouseholdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
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

                // 1. Stwórz domostwo
                var house = new DBHouse
                {
                    Name = name,
                    Admin = user,
                    Description = description,
                    AdminId = user.Id,
                    JoinCode = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper() // np. "A1B2C3"
                };
                db.Houses.Add(house);
                await db.SaveChangesAsync();

                // 2. Przypisz użytkownika do domu i ustaw jako admin
                user.HouseId = house.Id;
                user.Role = SystemRole.HouseholdAdmin;

                await db.SaveChangesAsync();

                return Results.Content("<div class='success'>Domostwo utworzone!</div>", "text/html");
            });

        }
    }
}
