using Xunit;
using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HomeBudgetManager.Tests
{
    public class RecurringTransactionTests
    {
        // Prawdziwy test (Integration Test) z bazą In-Memory
        [Fact]
        public void Calendar_Simulation_ShouldReturnUserTransactions()
        {
            // 1. ARRANGE (Konfiguracja fałszywej bazy)
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CalendarTest_Fixed_" + Guid.NewGuid())
                .Options;

            // Wrzucamy dane testowe
            using (var db = new AppDbContext(options))
            {
                // Tworzymy Usera (Poprawione: bez JoinCode, rola Guest)
                var user = new DBUser
                {
                    Id = 1,
                    Login = "Tester",
                    Email = "test@test.com",
                    Password = "hash",
                    Role = SystemRole.Guest, // Zmieniono z User na Guest
                    HouseId = null
                };
                db.Users.Add(user);

                // Tworzymy Kategorię
                var cat = new DBCategory { Id = 10, Name = "Praca", UserId = 1 };
                db.Categories.Add(cat);

                // Tworzymy Transakcję
                db.Transactions.Add(new DBTransaction
                {
                    Id = 100,
                    UserId = 1,
                    CategoryId = 10,
                    Value = 500.00m,
                    Date = DateTime.Now,
                    Title = "Test Transaction",
                    TransactionType = TransactionType.income,
                    Description = "Wypłata za testy"
                });

                db.SaveChanges();
            }

            // 2. ACT (Symulacja działania Kalendarza)
            using (var db = new AppDbContext(options))
            {
                // To jest logika wyjęta z CalendarEndpoint.cs
                var transactions = db.Transactions
                    .Where(t => t.UserId == 1)
                    .OrderBy(t => t.Date)
                    .ToList();

                // 3. ASSERT (Sprawdzamy czy działa)
                Assert.Single(transactions); // Czy znalazł 1 transakcję?
                Assert.Equal(500.00m, transactions[0].Value);
                Assert.Equal("Wypłata za testy", transactions[0].Description);
            }
        }


    }
}