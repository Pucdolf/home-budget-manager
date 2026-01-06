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
    public class DashboardHouseholdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/dashboard-household", async (HttpContext context, AppDbContext db) =>
            {
                var login = context.Request.Cookies["logged_user"];
                if (string.IsNullOrEmpty(login))
                    return Results.Text("Błąd: użytkownik niezalogowany", "text/plain");

                var user = await db.Users
                    .Include(u => u.user_house) // załaduj domostwo
                    .FirstOrDefaultAsync(u => u.user_login == login);

                if (user == null)
                    return Results.Text("Błąd: użytkownik nie istnieje", "text/plain");

                try
                {
                    if (user.user_house_id is null)
                    {
                        // użytkownik nie należy do domostwa
                        var html = $@"
            <section class='card'>
                <h2>Twoje domostwo</h2>
                <p>Nie jesteś jeszcze członkiem żadnego domostwa.</p>
                <div class='actions-box'>
                    <a href='createHousehold.html' class='btn-primary'>Utwórz domostwo</a>
                    <a href='joinHousehold.html' class='btn-primary'>Dołącz do domostwa</a>
                </div>
            </section>";
                        return Results.Content(html, "text/html");
                    }
                    else
                    {
                        // użytkownik ma domostwo
                        var house = user.user_house!;
                        var html = $@"
            <section class='card'>
                <h2>Twoje domostwo</h2>
                <p><strong>Nazwa:</strong> {house.house_name}</p>
                <p><strong>Opis:</strong> {house.house_description}</p>
                <p><strong>Admin ID:</strong> {house.house_admin_id}</p>
                <p><strong>Kod zaproszenia:</strong> {house.house_join_code}</p>
                <!-- Tu później dodasz np. listę członków -->
            </section>";
                        return Results.Content(html, "text/html");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd wczytania strony: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner: {ex.InnerException.Message}");
                    }
                    return Results.Content($"<div class='error'>Błąd serwera: nie udało się wczytać strony.</div>", "text/html");
                }
            });

        }
    }
}
