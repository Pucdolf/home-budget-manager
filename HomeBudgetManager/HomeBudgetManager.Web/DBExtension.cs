using HomeBudgetManager.Core;
using Microsoft.EntityFrameworkCore;

namespace HomeBudgetManager.Web
{
    public static class DBExtension
    {
        public static void UpdateDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    context.Database.Migrate();
                    Console.WriteLine("Database migrated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
                    throw;
                }
            }
        }
    }
}