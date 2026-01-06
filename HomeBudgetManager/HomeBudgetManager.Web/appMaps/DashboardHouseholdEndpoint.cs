using HomeBudgetManager.Core;
using Microsoft.EntityFrameworkCore;

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
                    .Include(u => u.House) // załaduj domostwo
                    .FirstOrDefaultAsync(u => u.Login == login);

                if (user == null)
                    return Results.Text("Błąd: użytkownik nie istnieje", "text/plain");

                try
                {
                    if (user.HouseId is null)
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
                        var house = user.House!;
                        var html = $@"
                            <section class='card'>
                                <h2>Twoje domostwo</h2>
                                <p><strong>Nazwa:</strong> {house.Name}</p>
                                <p><strong>Opis:</strong> {house.Description}</p>
                                <p><strong>Admin ID:</strong> {house.AdminId}</p>
                                <p><strong>Kod zaproszenia:</strong> {house.JoinCode}</p>
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
