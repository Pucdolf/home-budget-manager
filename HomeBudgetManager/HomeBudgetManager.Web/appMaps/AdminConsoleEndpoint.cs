// using System.Text;
// using Microsoft.EntityFrameworkCore;
// using HomeBudgetManager.Core;

// namespace HomeBudgetManager.Web.appMaps
// {
//     public class AdminConsoleEndpoint : IEndpoint
//     {
//         public void Map(IEndpointRouteBuilder app)
//         {
//             app.MapGet("/adminConsole", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
//             {
//                 if (!context.Request.Cookies.ContainsKey("logged_user"))
//                     return Results.Redirect("/");

//                 var username = context.Request.Cookies["logged_user"].ToString();
                
//                 // Pobieramy użytkownika
//                 var user = await db.Users.FirstOrDefaultAsync(u => u.Login == username);
                
//                 if (user == null)
//                     return Results.Redirect("/");

//                 // --- ZABEZPIECZENIE BACKENDU ---
//                 // Nawet jeśli ukryjesz przycisk, ktoś może wpisać adres z palca.
//                 // Tutaj sprawdzamy, czy użytkownik w ogóle może widzieć tę stronę.
//                 if (user.RoleId != SystemRole.SystemAdmin)
//                 {
//                     // Jeśli nie jest adminem, przekieruj go np. na pulpit
//                     return Results.Redirect("/dashboard"); 
//                 }

//                 var filePath = Path.Combine(env.WebRootPath, "adminConsole.html");
//                 var html = File.ReadAllText(filePath, Encoding.UTF8);

//                 // --- LOGIKA PRZYCISKU ---
//                 // Generujemy HTML przycisku tylko dla admina
//                 // (W tym konkretnym pliku user na pewno jest adminem przez if wyżej,
//                 // ale ten sam kod możesz skopiować do Dashboardu, gdzie user może być zwykłym członkiem)
//                 string adminBtnHtml = "";
                
//                 if (user.RoleId == SystemRole.SystemAdmin)
//                 {
//                     adminBtnHtml = "<button class=\"sidebar-link\" onclick=\"window.location.href='/adminConsole'\">Ustawienia Admina</button>";
//                 }

//                 // Podmieniamy placeholdery
//                 html = html.Replace("{username}", username);
//                 html = html.Replace("{admin_panel_button}", adminBtnHtml);

//                 return Results.Content(html, "text/html; charset=utf-8");
//             });
//         }
//     }
// }
