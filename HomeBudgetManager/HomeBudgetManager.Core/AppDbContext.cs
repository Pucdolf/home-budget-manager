using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HomeBudgetManager.Core.DBTables;

namespace HomeBudgetManager.Core
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DBHouse> Houses { get; set; }
        public DbSet<DBTransaction> Transactions { get; set; }
        public DbSet<DBCategory> Categories { get; set; }
        public DbSet<DBUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DBUser>()
                .HasOne(u => u.house)
                .WithMany(h => h.Members)
                .HasForeignKey(u => u.user_house_id)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
