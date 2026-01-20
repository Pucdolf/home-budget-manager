using HomeBudgetManager.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace HomeBudgetManager.Web.appMaps
{
    public class ChartsEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/charts", (HttpContext context) =>
            {
                if (!context.Request.Cookies.TryGetValue("logged_user", out var username) || string.IsNullOrEmpty(username))
                {
                    return Results.Redirect("/");
                }
                
                // Basic layout similar to dashboard
                // We'll use a simple form to pick dates, defaulting to current month
                var now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
                var endDate = now.ToString("yyyy-MM-dd");

                var html = $$"""
                <!DOCTYPE html>
                <html lang="pl">
                <head>
                    <meta charset="UTF-8">
                    <title>Wykresy - HomeBudgetManager</title>
                    <link rel="stylesheet" href="/css/dashboard.css">
                    <script src="https://unpkg.com/htmx.org@1.9.10"></script>
                    <style>
                        .charts-controls {
                            background: #fff;
                            padding: 20px;
                            border-radius: 8px;
                            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
                            margin-bottom: 20px;
                            display: flex;
                            gap: 15px;
                            align-items: end;
                        }
                        .form-group {
                            display: flex;
                            flex-direction: column;
                        }
                        .form-group label {
                            font-size: 0.9em;
                            margin-bottom: 5px;
                            color: #555;
                        }
                        .btn-generate {
                            padding: 10px 20px;
                            background-color: #124708;
                            color: white;
                            border: none;
                            border-radius: 4px;
                            cursor: pointer;
                        }
                        .btn-generate:hover {
                            background-color: #0e3606;
                        }
                    </style>
                </head>
                <body>
                    <div class="layout">
                        <aside class="sidebar">
                            <p class="sidebar-title">Mój budżet</p>
                            <span class="user-name">Zalogowano jako: {{username}}</span>
                            <button class="sidebar-link" onclick="window.location.href='/dashboard'">Pulpit</button>
                            <button class="sidebar-link" onclick="window.location.href='/dashboard-household'">Domostwo</button>
                            <button class="sidebar-link" onclick="window.location.href='/calendar'">Kalendarz</button>
                            <button class="sidebar-link active">Wykresy</button>
                            <form method="post" action="/logout" style="display:inline;">
                                <button type="submit" class="btn-logout">Wyloguj</button>
                            </form>
                        </aside>

                        <main class="container">
                            <section class="card">
                                <h2>Wykresy finansowe</h2>
                                <p class="card-desc">Wybierz zakres dat, aby wygenerować wykresy wydatków i przychodów.</p>

                                <form class="charts-controls" hx-post="/charts/generate" hx-target="#charts-result">
                                    <div class="form-group">
                                        <label for="startDate">Od:</label>
                                        <input type="date" id="startDate" name="startDate" value="{{startDate}}" required>
                                    </div>
                                    <div class="form-group">
                                        <label for="endDate">Do:</label>
                                        <input type="date" id="endDate" name="endDate" value="{{endDate}}" required>
                                    </div>
                                    <button type="submit" class="btn-generate">Generuj</button>
                                </form>

                                <div id="charts-result">
                                    <!-- Tu pojawią się wykresy -->
                                    <p style="text-align:center; color:#888;">Kliknij "Generuj", aby zobaczyć dane.</p>
                                </div>
                            </section>
                        </main>
                    </div>
                </body>
                </html>
                """;
                return Results.Content(html, "text/html");
            });

            app.MapPost("/charts/generate", (HttpContext context, ChartService chartService) =>
            {
                if (!context.Request.Cookies.TryGetValue("user_id", out var userIdString) || 
                    !int.TryParse(userIdString, out int userId))
                {
                    // Redirect to login or show error
                     return Results.Content("<div class='error'>Sesja wygasła. Zaloguj się ponownie.</div>", "text/html");
                }

                var form = context.Request.Form;
                if (!DateTime.TryParse(form["startDate"], out DateTime startDate) ||
                    !DateTime.TryParse(form["endDate"], out DateTime endDate))
                {
                     return Results.Content("<div class='error'>Nieprawidłowy format daty.</div>", "text/html");
                }
                
                // End date should include the whole day
                endDate = endDate.Date.AddDays(1).AddTicks(-1);

                var chartsHtml = chartService.GenerateChartsHtml(userId, startDate, endDate);
                return Results.Content(chartsHtml, "text/html");
            });
        }
    }
}
