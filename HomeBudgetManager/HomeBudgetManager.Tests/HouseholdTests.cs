using Xunit;
using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetManager.Tests
{
    public class HouseholdTests
    {
        // 1. SYMULACJA TWORZENIA DOMU (Logic from CreateHouseholdEndpoint)
        [Fact]
        public async Task CreateHousehold_Simulation_ShouldCreateHouse_And_PromoteUser()
        {
            // ARRANGE - Przygotowanie bazy
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "House_Sim_Create_" + Guid.NewGuid())
                .Options;

            using (var db = new AppDbContext(options))
            {
                // Mamy użytkownika "bezdomnego" (Guest)
                db.Users.Add(new DBUser
                {
                    Id = 1,
                    Login = "Ojciec",
                    Email = "t@t.com",
                    Password = "123",
                    Role = SystemRole.Guest,
                    HouseId = null
                });
                await db.SaveChangesAsync();
            }

            // ACT - Wykonujemy logikę żywcem wyjętą z CreateHouseholdEndpoint.cs
            using (var db = new AppDbContext(options))
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == 1);

                // Logika z endpointu:
                var house = new DBHouse
                {
                    Name = "Nasza Chata",
                    Admin = user,
                    Description = "Opis testowy",
                    AdminId = user.Id,
                    // Symulacja generowania kodu (jak w endpoincie)
                    JoinCode = "ABCDEF"
                };

                db.Houses.Add(house);
                await db.SaveChangesAsync(); // Zapis domu, żeby dostał ID

                // Aktualizacja usera (to co robi endpoint)
                user.HouseId = house.Id;
                user.Role = SystemRole.HouseholdAdmin;

                await db.SaveChangesAsync();
            }

            // ASSERT - Sprawdzamy czy baza wygląda tak, jak powinna
            using (var db = new AppDbContext(options))
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == 1);
                var house = await db.Houses.FirstOrDefaultAsync();

                Assert.NotNull(house);
                Assert.Equal("Nasza Chata", house.Name);

                // Czy User jest przypisany?
                Assert.Equal(house.Id, user.HouseId);

                // Czy User awansował na Admina Domu?
                Assert.Equal(SystemRole.HouseholdAdmin, user.Role);
            }
        }

        // 2. SYMULACJA DOŁĄCZANIA (Logic Simulation)
        [Fact]
        public async Task JoinHousehold_Simulation_ShouldAddUser_WhenCodeIsCorrect()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "House_Sim_Join_" + Guid.NewGuid())
                .Options;

            string secretCode = "XYZ123";


            using (var db = new AppDbContext(options))
            {
                // Tworzymy istniejący dom
                db.Houses.Add(new DBHouse { Id = 99, Name = "Dom Istniejący", JoinCode = secretCode });

                // Tworzymy usera, który chce dołączyć
                db.Users.Add(new DBUser
                {
                    Id = 2,
                    Login = "Syn",
                    Email = "s@s.com",
                    Password = "123",
                    Role = SystemRole.Guest,
                    HouseId = null
                });
                await db.SaveChangesAsync();
            }

            // ACT - Symulacja logiki dołączania (szukanie po kodzie)
            using (var db = new AppDbContext(options))
            {
                // Szukamy domu po kodzie
                var house = await db.Houses.FirstOrDefaultAsync(h => h.JoinCode == secretCode);
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == 2);

                if (house != null && user != null)
                {
                    user.HouseId = house.Id;
                    // Rola zostaje Guest (bo to tylko domownik), chyba że logika jest inna
                    await db.SaveChangesAsync();
                }
            }

            // ASSERT
            using (var db = new AppDbContext(options))
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Id == 2);
                Assert.Equal(99, user.HouseId); // Czy trafił do dobrego domu?
            }
        }
    }
}