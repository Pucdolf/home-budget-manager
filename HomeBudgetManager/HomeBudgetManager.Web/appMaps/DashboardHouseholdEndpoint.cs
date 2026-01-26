using System.Net;
using System.Text;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;

namespace HomeBudgetManager.Web.appMaps
{
    public class DashboardHouseholdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/household", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
            {
                if (!context.Request.Cookies.ContainsKey("logged_user"))
                    return Results.Redirect("/");
                var userId = int.Parse(context.Request.Cookies["user_id"]);
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                var username = context.Request.Cookies["logged_user"];
                if (user == null)
                    return Results.Redirect("/");

                var filePath = Path.Combine(env.WebRootPath, "household.html");
                var html = File.ReadAllText(filePath, Encoding.UTF8);
                string adminBtnHtml = "";
                
                if (user.Role == SystemRole.SystemAdmin)
                {
                    adminBtnHtml = "<button class=\"sidebar-link\" onclick=\"window.location.href='/adminConsole'\"><i class=\"fas fa-fw fa-cogs\"></i> &nbsp; Ustawienia Admina</button>";
                }
                html = html.Replace("{username}", username)
                .Replace("{admin_panel_button}", adminBtnHtml);

                return Results.Content(html, "text/html; charset=utf-8");
            });

            app.MapGet("/dashboard-household", async (HttpContext context, AppDbContext db) =>
            {
                var login = context.Request.Cookies["logged_user"];
                if (string.IsNullOrEmpty(login))
                    return Results.Text("Błąd: użytkownik niezalogowany", "text/plain");

                return await RenderHouseholdView(db, login);
            });

            app.MapPost("/remove-member", async (int userId, HttpContext context, AppDbContext db) =>
            {
                var login = context.Request.Cookies["logged_user"];
                if (string.IsNullOrEmpty(login))
                    return Results.Text("Błąd: użytkownik niezalogowany", "text/plain");

                var adminUser = await db.Users.FirstOrDefaultAsync(u => u.Login == login);
                if (adminUser == null || adminUser.Role != SystemRole.HouseholdAdmin || adminUser.HouseId == null)
                    return Results.Text("Błąd: brak uprawnień", "text/plain");

                var targetUser = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (targetUser == null || targetUser.HouseId != adminUser.HouseId)
                    return Results.Text("Błąd: użytkownik nie należy do Twojego domostwa", "text/plain");

                // Delete user
                targetUser.HouseId = null;
                targetUser.Role = SystemRole.Guest;
                await db.SaveChangesAsync();

                return await RenderHouseholdView(db, login);
            });
        }

        private static async Task<IResult> RenderHouseholdView(AppDbContext db, string login)
        {
            var user = await db.Users
                .Include(u => u.House)
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user == null)
                return Results.Text("Błąd: użytkownik nie istnieje", "text/plain");

            try
            {
                string cssLink = "<link rel='stylesheet' href='/css/householdView.css'>";

                if (user.HouseId is null)
                {
                    var html = $@"
                        {cssLink}
                        <section class='card' style='display: flex; width: 600px; justify-content: center;'>
                            <h2>Twoje domostwo</h2>
                            <p>Nie jesteś jeszcze członkiem żadnego domostwa.</p>
                            <div class='actions-box'>
                                <a href='createHousehold.html' class='btn-primary'>Utwórz domostwo</a>
                                <a href='joinHousehold.html' class='btn-primary'>Dołącz do domostwa</a>
                            </div>
                        </section>
                        <div style='margin-top: 2rem; display: flex; gap: 1rem;'>
                            <div class='ad-placeholder' style='flex: 1; height: 400px;'><span>Miejsce na reklamę</span></div>
                            <div class='ad-placeholder' style='flex: 1; height: 400px;'><span>Miejsce na reklamę</span></div>
                        </div>";
                    return Results.Content(html, "text/html");
                }
                else
                {
                    var house = user.House!;
                    bool iAmAdmin = user.Role == SystemRole.HouseholdAdmin;

                    var confirmText = iAmAdmin
                        ? "Jako administrator, opuszczając domostwo, spowodujesz jego trwałe usunięcie. Czy na pewno chcesz kontynuować?"
                        : "Czy na pewno chcesz opuścić domostwo?";

                    var buttonClass = "btn-danger";

                    // sort:  Admin on top, the rest alphabetically
                    var members = await db.Users
                        .Where(u => u.HouseId == house.Id)
                        .OrderByDescending(u => u.Role == SystemRole.HouseholdAdmin)
                        .ThenBy(u => u.Login)
                        .ToListAsync();

                    var rowsBuilder = new StringBuilder();

                    if (members.Count == 0)
                    {
                        rowsBuilder.AppendLine("<tr><td colspan='5'>Brak członków w tym domostwie.</td></tr>");
                    }
                    else
                    {
                        foreach (var m in members)
                        {
                            var isMe = string.Equals(m.Login, user.Login, StringComparison.OrdinalIgnoreCase);
                            var isTargetAdmin = m.Role == SystemRole.HouseholdAdmin;

                            var loginEsc = WebUtility.HtmlEncode(m.Login);
                            var emailEsc = WebUtility.HtmlEncode(m.Email);

                            string roleDisplay;
                            if (isTargetAdmin) roleDisplay = "Administrator";
                            else roleDisplay = "Członek";

                            var rowClass = isMe ? "member-row current-user" : "member-row";
                            var loginDisplay = isMe ? $"{loginEsc} (Ty)" : loginEsc;

                            string actionCell = "";

                            if (iAmAdmin && !isTargetAdmin)
                            {
                                actionCell = $@"
                                    <button 
                                        class='removeBtn'
                                        hx-post='/remove-member?userId={m.Id}'
                                        hx-confirm='Czy na pewno chcesz usunąć użytkownika {loginEsc}?'
                                        hx-target='#household-main'>
                                        Usuń
                                    </button>";
                            }
                            else if (!iAmAdmin)
                            {
                                
                                actionCell = "<span style='color:#ccc; font-size:0.8em;'>(brak uprawnień)</span>";
                            }

                            else if(iAmAdmin && isTargetAdmin)
                            {
                                
                                actionCell = "<span style='color:#ccc; font-size:0.8em;'>-</span>";
                            }

                            rowsBuilder.AppendLine($@"
                                <tr class='{rowClass}'>
                                    <td>{m.Id}</td>
                                    <td>{loginDisplay}</td>
                                    <td>{emailEsc}</td>
                                    <td>{roleDisplay}</td>
                                    <td style='text-align:center;'>{actionCell}</td>
                                </tr>");
                        }
                    }

                    var html = $@"
                    {cssLink}
                    
                    <h1 class='page-title' style='margin-left: 0px !important;'>Twoje domostwo</h1>

                    <div class='main-card'>
                        
                        <div class='card'>
                            <h3 class='card-header'>Szczegóły</h3>
                            
                            <div class='info-row'>
                                <span class='info-label'>Nazwa:</span>
                                <span class='info-value'>{WebUtility.HtmlEncode(house.Name)}</span>
                            </div>

                            <div class='info-row'>
                                <span class='info-label'>Opis:</span>
                                <span class='info-value'>{WebUtility.HtmlEncode(house.Description ?? string.Empty)}</span>
                            </div>

                            <div class='info-row'>
                                <span class='info-label'>Kod zaproszenia:</span>
                                <span class='info-value' style='font-family: monospace; font-size: 1.2em;'>{WebUtility.HtmlEncode(house.JoinCode)}</span>
                            </div>

                            <div>
                                <form 
                                    hx-post='/leave-household'
                                    hx-target='#household-main'
                                    hx-swap='innerHTML'
                                    hx-confirm='{confirmText}'>
                                    
                                    <button type='submit' class='{buttonClass}'>
                                        Opuść domostwo
                                    </button>
                                </form>
                            </div>
                        </div>

                        <div class='members-card'>
                            <h3 class='card-header'>Członkowie domostwa</h3>

                            <table class='members-table'>
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Login</th>
                                            <th>Email</th>
                                            <th>Rola</th>
                                            <th>Akcje</th> </tr>
                                    </thead>
                                    <tbody>
                                        {rowsBuilder}
                                    </tbody>
                            </table>
                        </div>

                    </div>
                    
                    <div style='margin-top: 2rem; display: flex; gap: 1rem;'>
                        <div class='ad-placeholder' style='flex: 1; height: 150px;'><span>Miejsce na reklamę</span></div>
                        <div class='ad-placeholder' style='flex: 1; height: 150px;'><span>Miejsce na reklamę</span></div>
                    </div>";

                    return Results.Content(html, "text/html");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd wczytania strony: {ex.Message}");
                return Results.Content("<div class='error'>Błąd serwera: nie udało się wczytać strony.</div>", "text/html");
            }
        }
    }
}