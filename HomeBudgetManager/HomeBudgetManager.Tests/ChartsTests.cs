using Xunit;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HomeBudgetManager.Tests
{
    public class ChartsTests
    {
        [Fact]
        public void GetStatistics_ShouldCalculateTotalsAndPercentages_Correctly()
        {
            // 1. ARRANGE - Przygotuj bazę danych (In-Memory)
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "ChartsTestDb_" + Guid.NewGuid())
                .Options;

            using (var db = new AppDbContext(options))
            {
                // Dodajemy Usera
                db.Users.Add(new DBUser { Id = 1, Login = "Kass", Email = "k@k.pl", Password = "123", Role = SystemRole.Guest, HouseId = null });

                // Dodajemy Kategorie
                var catFood = new DBCategory { Id = 1, Name = "Jedzenie", UserId = 1 };
                var catFuel = new DBCategory { Id = 2, Name = "Paliwo", UserId = 1 };
                db.Categories.AddRange(catFood, catFuel);

                // Dodajemy Transakcje (W tym miesiącu)
                db.Transactions.AddRange(
                    // 2x Jedzenie po 50zł = 100zł
                    new DBTransaction { UserId = 1, CategoryId = 1, Value = -50m, TransactionType = TransactionType.expense, Date = DateTime.Now, Title = "Obiad" },
                    new DBTransaction { UserId = 1, CategoryId = 1, Value = -50m, TransactionType = TransactionType.expense, Date = DateTime.Now, Title = "Kolacja" },

                    // 1x Paliwo po 50zł = 50zł
                    new DBTransaction { UserId = 1, CategoryId = 2, Value = -50m, TransactionType = TransactionType.expense, Date = DateTime.Now, Title = "Paliwo" }
                );

                // Dodajemy transakcję spoza zakresu (STARY ROK) - nie powinna być policzona!
                db.Transactions.Add(
                    new DBTransaction { UserId = 1, CategoryId = 1, Value = -999m, TransactionType = TransactionType.expense, Date = DateTime.Now.AddYears(-2), Title = "Stare" }
                );

                db.SaveChanges();
            }

            // 2. ACT - Uruchom logikę wykresów
            using (var db = new AppDbContext(options))
            {
                var service = new ChartService(db);
                var userIds = new List<int> { 1 };

                // Pytamy o statystyki z ostatniego miesiąca (omijając tę starą transakcję)
                var startDate = DateTime.Now.AddDays(-10);
                var endDate = DateTime.Now.AddDays(10);

                var (expenses, incomes) = service.GetStatistics(userIds, startDate, endDate);

                // 3. ASSERT - Sprawdź wyniki (Matematyka)

                // Powinniśmy mieć 2 kategorie w wydatkach (Jedzenie, Paliwo)
                Assert.Equal(2, expenses.Count);

                // Sprawdź Jedzenie (100 zł)
                var foodStat = expenses.First(x => x.CategoryName == "Jedzenie");
                Assert.Equal(100m, foodStat.TotalAmount);

                // Sprawdź Paliwo (50 zł)
                var fuelStat = expenses.First(x => x.CategoryName == "Paliwo");
                Assert.Equal(50m, fuelStat.TotalAmount);

                // Sprawdź Procenty (Jedzenie to 100 z 150 = 66.66%)
                // Tolerancja 1% na zaokrąglenia
                Assert.InRange(foodStat.Percentage, 66, 67);
            }
        }

        [Fact]
        public void GenerateChartsHtml_ShouldReturnString_WhenDataExists()
        {
            // Szybki test sprawdzający, czy metoda generująca HTML się nie wysypuje
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: "ChartsHtmlTest_" + Guid.NewGuid())
               .Options;

            using (var db = new AppDbContext(options))
            {
                var service = new ChartService(db);
                // Przekazujemy pustą bazę - powinien zwrócić HTML z komunikatem "Brak danych"
                var html = service.GenerateChartsHtml(1, DateTime.Now, DateTime.Now);

                Assert.NotNull(html);
                Assert.Contains("Brak danych", html); // Sprawdzamy czy backend ładnie obsłużył pustkę
            }
        }
    }
}