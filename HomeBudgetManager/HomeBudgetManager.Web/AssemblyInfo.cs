using HomeBudgetManager.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HomeBudgetManager.Web
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlite("Data Source=HbmLocal.db", b => b.MigrationsAssembly("HomeBudgetManager.Core"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}