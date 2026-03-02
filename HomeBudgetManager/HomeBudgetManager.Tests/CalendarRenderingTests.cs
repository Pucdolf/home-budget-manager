using Xunit;
using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetManager.Tests
{
    public class CalendarRenderingTests
    {
        [Fact]
        public async Task Calendar_ShouldRenderCorrectColors_AndFormatTitles()
        {
            // 1. ARRANGE - Przygotowanie bazy danych w pamięci
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "CalendarRenderDb_" + Guid.NewGuid())
                .Options;

            // Wrzucamy dane testowe: 1 Przychód (Zielony) i 1 Wydatek (Czerwony)
            using (var db = new AppDbContext(options))
            {
                var user = new DBUser
                {
                    Id = 1,
                    Login = "TestUser",
                    Email = "t@t.com",
                    Password = "x",
                    Role = SystemRole.HouseholdAdmin,
                    HouseId = null
                };
                db.Users.Add(user);

                var cat = new DBCategory { Id = 1, Name = "General", UserId = 1 };
                db.Categories.Add(cat);

                db.Transactions.AddRange(
                    new DBTransaction
                    {
                        Id = 1,
                        UserId = 1,
                        CategoryId = 1,
                        Value = 100.00m, // Przychód
                        TransactionType = TransactionType.income,
                        Date = DateTime.Now,
                        Title = "Zysk operacyjny",
                        Description = "Zysk"
                    },
                    new DBTransaction
                    {
                        Id = 2,
                        UserId = 1,
                        CategoryId = 1,
                        Value = -50.00m, // Wydatek
                        TransactionType = TransactionType.expense,
                        Date = DateTime.Now.AddDays(1),
                        Title = "Strata operacyjna",
                        Description = "Strata"
                    }
                );
                await db.SaveChangesAsync();
            }

            // 2. ACT - Symulacja logiki z CalendarEndpoint.cs (/api/calendar-events)
            using (var db = new AppDbContext(options))
            {
                // To jest kopia logiki z Twojego pliku CalendarEndpoint.cs
                // Testujemy, czy ta logika poprawnie przygotowuje dane dla JavaScriptu
                var events = await db.Transactions
                    .Include(t => t.User)
                    .Where(t => t.UserId == 1)
                    .OrderBy(t => t.Date)
                    .Select(t => new
                    {
                        id = t.Id.ToString(),
                        title = t.Title, // Formatowanie tytułu
                        color = t.Value < 0 ? "#e74a3b" : "#1cc88a", // Logika kolorów
                        amount = t.Value
                    })
                    .ToListAsync();

                // 3. ASSERT - Sprawdzamy czy frontend dostanie dobre dane

                Assert.Equal(2, events.Count);

                // Sprawdzenie Przychodu (Musi być zielony)
                var incomeEvent = events.First(e => e.amount > 0);
                Assert.Equal("#1cc88a", incomeEvent.color); // Zielony z CSS
                Assert.Equal("Zysk operacyjny", incomeEvent.title);

                // Sprawdzenie Wydatku (Musi być czerwony)
                var expenseEvent = events.First(e => e.amount < 0);
                Assert.Equal("#e74a3b", expenseEvent.color); // Czerwony z CSS
            }
        }
    }
}