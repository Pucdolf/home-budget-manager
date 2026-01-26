using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore;
namespace HomeBudgetManager.Web.appMaps
{
    public class DashboardEndpoints : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/dashboard", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
            {
                // check login status
                var userId = int.Parse(context.Request.Cookies["user_id"]);
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);



                var username = context.Request.Cookies["logged_user"];

                if (user == null)
                {
                    return Results.Redirect("/");
                }

                
                var balance = await db.Transactions
                                    .Where(t => t.UserId == user.Id)
                                    .SumAsync(t => t.Value);
                

                var filePath = Path.Combine(env.WebRootPath, "dashboard.html");
                // load html
                var html = File.ReadAllText(filePath);
                string adminBtnHtml = "";
                
                if (user.Role == SystemRole.SystemAdmin)
                {
                    adminBtnHtml = "<button class=\"sidebar-link\" onclick=\"window.location.href='/adminConsole'\"><i class=\"fas fa-fw fa-cogs\"></i> &nbsp; Ustawienia Admina</button>";
                }
                // replace placeholders with correct values
                html =  html.Replace("{username}", username)
                            .Replace("{balance}", balance.ToString("N2"))
                            .Replace("{admin_panel_button}", adminBtnHtml);
                return Results.Content(html, "text/html");
            });

            app.MapGet("/dashboard/charts", (HttpContext context, ChartService chartService) =>
            {
                var userIdentifier = context.Request.Cookies["user_id"];
                if (string.IsNullOrEmpty(userIdentifier))
                {
                    return Results.Content("");
                }
                int userId =int.Parse(userIdentifier);
                string chartsHtml = chartService.GenerateDashboardChartsHtml(userId);
                

                return Results.Content(chartsHtml, "text/html");
            });
        }
    }
}