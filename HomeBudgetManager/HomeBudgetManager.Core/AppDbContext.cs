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
        public DbSet<DBRole> Roles { get; set; }

        public DbSet<DBRepetableTransaction> RepetableTransactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DBUser>()
                .HasOne(u => u.House)
                .WithMany(h => h.Members)
                .HasForeignKey(u => u.HouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DBTransaction>()
                .HasOne(a => a.RepetableTransaction) // Jeśli masz to pole w DBTransaction
                .WithOne()         // Wskazujemy na właściwość w DBRepetableTransaction
                .HasForeignKey<DBRepetableTransaction>(b => b.TransactionId); // TU JEST KLUCZ: Wymuszamy użycie TransactionId

            // Seed Roles
            modelBuilder.Entity<DBRole>().HasData(
                Enum.GetValues(typeof(SystemRole))
                    .Cast<SystemRole>()
                    .Select(r => new DBRole
                    {
                        Id = r,
                        Name = r.ToString()
                    })
            );
        }

    }
}
