using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;

namespace HomeBudgetManager.Web.appMaps
{
    public class LeaveHouseholdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/leave-household", async (HttpContext context, AppDbContext db) =>
            {
                // Pobranie zalogowanego użytkownika
                var login = context.Request.Cookies["logged_user"];
                if (string.IsNullOrEmpty(login))
                {
                    return Results.Content("<div class='error'>Błąd: użytkownik niezalogowany.</div>", "text/html");
                }

                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == login);
                if (user == null)
                {
                    return Results.Content("<div class='error'>Błąd: użytkownik nie istnieje.</div>", "text/html");
                }
                if (user.HouseId == null)
                {
                    return Results.Content("<div class='error'>Nie należysz do żadnego domostwa.</div>", "text/html");
                }

                // Pobranie domu
                var house = await db.Houses.FirstOrDefaultAsync(h => h.Id == user.HouseId);
                if (house == null)
                {
                    return Results.Content("<div class='error'>Domostwo nie istnieje.</div>", "text/html");
                }

                // Jeśli użytkownik jest administratorem domu
                if (user.Role == SystemRole.HouseholdAdmin)
                {
                    // 1. Odłącz transakcje od usuwanego domu (ustaw HouseId = null)
                    var houseTransactions = await db.Transactions
                        .Where(t => t.HouseId == house.Id)
                        .ToListAsync();

                    foreach (var t in houseTransactions)
                    {
                        t.HouseId = null;
                    }

                    // 2. Reset dla wszystkich członków
                    var members = await db.Users.Where(u => u.HouseId == house.Id).ToListAsync();
                    foreach (var member in members)
                    {
                        member.HouseId = null;
                        member.Role = SystemRole.Guest;
                    }

                    // 3. Usuń dom
                    db.Houses.Remove(house);
                    await db.SaveChangesAsync();

                    return Results.Content(@"
                        <section class='card'>
                            <h2>Twoje domostwo</h2>
                            <div class='success'>Opuszczono domostwo (usunięto domostwo)!</div>
                            <p>Za chwilę nastąpi powrót do pulpitu...</p>
                        </section>
                        <script>
                            setTimeout(() => { window.location.href = '/dashboard'; }, 1200);
                        </script>
                    ", "text/html");

                }
                else
                {
                    // Zwykły członek opuszcza domostwo
                    int houseId = user.HouseId.Value;
                    user.HouseId = null;
                    
                    if (user.Role!=SystemRole.SystemAdmin)user.Role = SystemRole.Guest;
                    await db.SaveChangesAsync();

                    // Jeśli po jego odejściu dom jest pusty, usuń go
                    bool anyLeft = await db.Users.AnyAsync(u => u.HouseId == houseId);
                    if (!anyLeft)
                    {
                        db.Houses.Remove(house);
                        await db.SaveChangesAsync();
                    }

                    return Results.Content(@"
                        <section class='card'>
                            <h2>Twoje domostwo</h2>
                            <div class='success'>Opuszczono domostwo!</div>
                            <p>Za chwilę nastąpi powrót do pulpitu...</p>
                        </section>
                        <script>
                            setTimeout(() => { window.location.href = '/dashboard'; }, 1200);
                        </script>
                    ", "text/html");

                }
            });
        }
    }
}
