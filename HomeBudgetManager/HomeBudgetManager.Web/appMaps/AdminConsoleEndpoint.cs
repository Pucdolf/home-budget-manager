using System.Text;
using Microsoft.EntityFrameworkCore;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
namespace HomeBudgetManager.Web.appMaps
{
    public class AdminConsoleEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/adminConsole", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
            {
                if (!context.Request.Cookies.ContainsKey("logged_user"))
                    return Results.Redirect("/");

                var username = context.Request.Cookies["logged_user"].ToString();
                
                // Pobieramy użytkownika
                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == username);
                
                if (user == null)
                    return Results.Redirect("/");

                // --- ZABEZPIECZENIE BACKENDU ---
                // Nawet jeśli ukryjesz przycisk, ktoś może wpisać adres z palca.
                // Tutaj sprawdzamy, czy użytkownik w ogóle może widzieć tę stronę.
                if (user.Role != SystemRole.SystemAdmin)
                {
                    // Jeśli nie jest adminem, przekieruj go np. na pulpit
                    return Results.Redirect("/dashboard"); 
                }

                var filePath = Path.Combine(env.WebRootPath, "adminConsole.html");
                var html = File.ReadAllText(filePath, Encoding.UTF8);

                // --- LOGIKA PRZYCISKU ---
                // Generujemy HTML przycisku tylko dla admina
                // (W tym konkretnym pliku user na pewno jest adminem przez if wyżej,
                // ale ten sam kod możesz skopiować do Dashboardu, gdzie user może być zwykłym członkiem)
                string adminBtnHtml = "";
                
                if (user.Role == SystemRole.SystemAdmin)
                {
                    adminBtnHtml = "<button class=\"sidebar-link\" onclick=\"window.location.href='/adminConsole'\"><i class=\"fas fa-fw fa-cogs\"></i> &nbsp; Ustawienia Admina</button>";
                }

                // Podmieniamy placeholdery
                html = html.Replace("{username}", username);
                html = html.Replace("{admin_panel_button}", adminBtnHtml);

                return Results.Content(html, "text/html; charset=utf-8");
            });

            app.MapPost("/admin/execute-sql", async (HttpContext context, AppDbContext db) =>
            {
                // -- Zabezpieczenie: Tylko admin może to wykonać --
                if (!context.Request.Cookies.TryGetValue("logged_user", out var username)) 
                    return Results.Content("<div class='error-msg'>Brak sesji.</div>");

                var user = await db.Users.FirstOrDefaultAsync(u => u.Login == username);
                if (user == null || user.Role != SystemRole.SystemAdmin) 
                    return Results.Content("<div class='error-msg'>Brak uprawnień administratora.</div>");

                // Pobranie zapytania z formularza
                var form = await context.Request.ReadFormAsync();
                string sqlQuery = form["sqlQuery"];

                if (string.IsNullOrWhiteSpace(sqlQuery))
                    return Results.Content("<div class='error-msg'>Zapytanie jest puste.</div>");

                StringBuilder sb = new StringBuilder();

                try
                {
                    // Używamy surowego połączenia ADO.NET, aby obsługiwać dowolne kolumny
                    var connection = db.Database.GetDbConnection();
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sqlQuery;
                        
                        // Sprawdzamy czy to SELECT czy coś innego (UPDATE/INSERT/DELETE)
                        if (sqlQuery.Trim().ToUpper().StartsWith("SELECT"))
                        {
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                sb.Append("<table class='sql-result-table'><thead><tr>");
                                
                                // Generuj nagłówki kolumn
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    sb.Append($"<th>{reader.GetName(i)}</th>");
                                }
                                sb.Append("</tr></thead><tbody>");

                                // Generuj wiersze
                                while (await reader.ReadAsync())
                                {
                                    sb.Append("<tr>");
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        var val = reader.GetValue(i);
                                        sb.Append($"<td>{val?.ToString() ?? "NULL"}</td>");
                                    }
                                    sb.Append("</tr>");
                                }
                                sb.Append("</tbody></table>");
                            }
                        }
                        else
                        {
                            // Dla UPDATE, DELETE, INSERT
                            int rowsAffected = await command.ExecuteNonQueryAsync();
                            sb.Append($"<div class='success-msg'>Zapytanie wykonane pomyślnie. Zodyfikowano wierszy: {rowsAffected}</div>");
                        }
                    }
                }
                catch (Exception ex)
                {
                    sb.Append($"<div class='error-msg'>Błąd SQL: {ex.Message}</div>");
                }

                return Results.Content(sb.ToString(), "text/html");
            });
        }
    }
}
