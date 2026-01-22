using HomeBudgetManager.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Claims;

namespace HomeBudgetManager.Web.appMaps
{
    public class ReportsEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/reports", (HttpContext context) =>
            {
                if (!context.Request.Cookies.TryGetValue("logged_user", out var username) || string.IsNullOrEmpty(username))
                {
                    return Results.Redirect("/");
                }

                var now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1).ToString("yyyy-MM-dd");
                var endDate = now.ToString("yyyy-MM-dd");

                var html = $$"""
                <!DOCTYPE html>
                <html lang="pl">
                <head>
                    <meta charset="UTF-8">
                    <title>Raporty - HomeBudgetManager</title>
                    <link rel="stylesheet" href="/css/dashboard.css">
                    <style>
                        .report-controls {
                            background: #fff;
                            padding: 20px;
                            border-radius: 8px;
                            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
                            margin-bottom: 20px;
                            display: flex;
                            gap: 30px;
                            align-items: flex-end;
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
                            background-color: #005f73;
                            color: white;
                            border: none;
                            border-radius: 4px;
                            cursor: pointer;
                            text-decoration: none;
                            display: inline-block;
                            height: fit-content;
                            margin-bottom: 22px;
                        }
                        .btn-generate:hover {
                            background-color: #0a4c5e;
                        }
                    </style>
                </head>
                <body>
                    <div class="layout">
                        <aside class="sidebar">
                            <p class="sidebar-title">Mój budżet</p>
                            <span class="user-name">Zalogowano jako: {{username}}</span>
                            <button class="sidebar-link" onclick="window.location.href='/dashboard'">Pulpit</button>
                            <button class="sidebar-link" onclick="window.location.href='/household'">Domostwo</button>
                            <button class="sidebar-link" onclick="window.location.href='/calendar'">Kalendarz</button>
                            <button class="sidebar-link" onclick="window.location.href='/charts'">Wykresy</button>
                            <button class="sidebar-link active" onclick="window.location.href='/reports'">Raporty</button>
                            <form method="post" action="/logout" style="display:inline;">
                                <button type="submit" class="btn-logout">Wyloguj</button>
                            </form>
                        </aside>

                        <main class="container">
                            <section class="card">
                                <h2>Generator Raportów PDF</h2>
                                <p class="card-desc">Wybierz zakres dat i wygeneruj szczegółowy raport PDF dla swojego domostwa.</p>

                                <form class="report-controls" method="post" action="/reports/generate">
                                    <div class="form-group">
                                        <label for="startDate">Od:</label>
                                        <input type="date" id="startDate" name="startDate" value="{{startDate}}" required>
                                    </div>
                                    <div class="form-group">
                                        <label for="endDate">Do:</label>
                                        <input type="date" id="endDate" name="endDate" value="{{endDate}}" required>
                                    </div>
                                    <button type="submit" class="btn-generate">Pobierz Raport PDF</button>
                                </form>
                            </section>
                        </main>
                    </div>
                </body>
                </html>
                """;
                return Results.Content(html, "text/html");
            });

            app.MapPost("/reports/generate", (HttpContext context, ReportService reportService) =>
            {
                if (!context.Request.Cookies.TryGetValue("user_id", out var userIdString) || 
                    !int.TryParse(userIdString, out int userId))
                {
                    return Results.Redirect("/");
                }

                var form = context.Request.Form;
                if (!DateTime.TryParse(form["startDate"], out DateTime startDate) ||
                    !DateTime.TryParse(form["endDate"], out DateTime endDate))
                {
                     return Results.Content("Nieprawidłowa data");
                }
                
                // End of day
                endDate = endDate.Date.AddDays(1).AddTicks(-1);

                var pdfBytes = reportService.GeneratePdfReport(userId, startDate, endDate);
                var filename = $"Raport_HBM_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf";

                return Results.File(pdfBytes, "application/pdf", filename);
            });
        }
    }
}
