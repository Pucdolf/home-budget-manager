using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Web.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

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

                var user = await db.Users.FirstOrDefaultAsync(u => u.user_login == userLogin);
                if (user == null)
                {
                    return Results.Content("<div class='error'>Błąd: użytkownik niezalogowany.</div>", "text/html");
                }

                if (user.user_house_id != null)
                {
                    return Results.Content("<div class='error'>Błąd: użytkownik należy już do domostwa.</div>", "text/html");
                }

                // 1. Stwórz domostwo
                var house = new DBHouse
                {
                    house_name = name,
                    house_description = description,
                    house_admin_id = user.user_id,
                    house_join_code = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper() // np. "A1B2C3"
                };
                db.Houses.Add(house);
                await db.SaveChangesAsync();

                // 2. Przypisz użytkownika do domu i ustaw jako admin
                user.user_house_id = house.house_id;
                user.user_role = SystemRole.HouseholdAdmin;

                await db.SaveChangesAsync();

                return Results.Content("<div class='success'>Domostwo utworzone!</div>", "text/html");
            });

        }
    }
}
