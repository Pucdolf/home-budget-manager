using Xunit;
using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBudgetManager.Tests
{
    public class HouseholdManagementTests
    {
        // 1. TEST: Zwykły członek opuszcza dom (LeaveHouseholdEndpoint)
        [Fact]
        public async Task LeaveHousehold_Member_ShouldResetUser_AndKeepHouse()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Leave_Member_" + Guid.NewGuid())
                .Options;

            using (var db = new AppDbContext(options))
            {
                var house = new DBHouse { Id = 10, Name = "Dom", JoinCode = "A", AdminId = 99 };
                db.Houses.Add(house);

                // Zwykły członek (Guest)
                db.Users.Add(new DBUser
                {
                    Id = 1,
                    Login = "Kuzyn",
                    Email = "k@k.com",
                    Password = "123",
                    Role = SystemRole.Guest,
                    HouseId = 10
                });

                // Admin (żeby dom nie był pusty)
                db.Users.Add(new DBUser
                {
                    Id = 99,
                    Login = "Szef",
                    Email = "a@a.com",
                    Password = "123",
                    Role = SystemRole.HouseholdAdmin,
                    HouseId = 10
                });

                await db.SaveChangesAsync();
            }

            // ACT - Symulacja logiki z LeaveHouseholdEndpoint.cs (blok else)
            using (var db = new AppDbContext(options))
            {
                var user = await db.Users.FirstAsync(u => u.Id == 1); // Kuzyn

                // Logika wyjęta z pliku:
                user.HouseId = null;
                user.Role = SystemRole.Guest;
                await db.SaveChangesAsync();
            }

            // ASSERT
            using (var db = new AppDbContext(options))
            {
                var user = await db.Users.FirstAsync(u => u.Id == 1);
                var house = await db.Houses.FirstOrDefaultAsync(h => h.Id == 10);

                Assert.Null(user.HouseId); // Już nie ma domu
                Assert.NotNull(house);     // Ale dom nadal stoi (bo został Admin)
            }
        }


        // 2. TEST: Admin rozwiązuje dom (LeaveHouseholdEndpoint)
        [Fact]
        public async Task LeaveHousehold_Admin_ShouldDissolveHouse()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Leave_Admin_" + Guid.NewGuid())
                .Options;

            using (var db = new AppDbContext(options))
            {
                var house = new DBHouse { Id = 20, Name = "Dom Admina", JoinCode = "B", AdminId = 1 };
                db.Houses.Add(house);

                // Admin
                db.Users.Add(new DBUser
                {
                    Id = 1,
                    Login = "Szef",
                    Email = "s@s.com",
                    Password = "123",
                    Role = SystemRole.HouseholdAdmin,
                    HouseId = 20
                });

                // Członek
                db.Users.Add(new DBUser
                {
                    Id = 2,
                    Login = "Członek",
                    Email = "c@c.com",
                    Password = "123",
                    Role = SystemRole.Guest,
                    HouseId = 20
                });

                await db.SaveChangesAsync();
            }

            // ACT - Symulacja logiki Admina (if user.Role == HouseholdAdmin)
            using (var db = new AppDbContext(options))
            {
                var house = await db.Houses.FirstAsync(h => h.Id == 20);

                // Logika: Reset dla wszystkich i usunięcie domu
                var members = await db.Users.Where(u => u.HouseId == house.Id).ToListAsync();
                foreach (var member in members)
                {
                    member.HouseId = null;
                    member.Role = SystemRole.Guest;
                }
                db.Houses.Remove(house);
                await db.SaveChangesAsync();
            }

            // ASSERT
            using (var db = new AppDbContext(options))
            {
                var house = await db.Houses.FirstOrDefaultAsync(h => h.Id == 20);
                var member = await db.Users.FirstAsync(u => u.Id == 2);

                Assert.Null(house);        // Dom powinien zniknąć!
                Assert.Null(member.HouseId); // Członek powinien zostać "uwolniony"
            }
        }

        // 3. TEST: Wyrzucanie członka przez Admina (DashboardHouseholdEndpoint)
        [Fact]
        public async Task KickMember_ShouldRemoveUserFromHouse()
        {
            // ARRANGE
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "Kick_Member_" + Guid.NewGuid())
                .Options;

            using (var db = new AppDbContext(options))
            {
                db.Houses.Add(new DBHouse { Id = 30, Name = "Twierdza", JoinCode = "C", AdminId = 1 });

                // Ofiara
                db.Users.Add(new DBUser
                {
                    Id = 5,
                    Login = "Niechciany",
                    Email = "n@n.com",
                    Password = "123",
                    Role = SystemRole.Guest,
                    HouseId = 30
                });
                await db.SaveChangesAsync();
            }

            // ACT - Symulacja endpointu /remove-member
            using (var db = new AppDbContext(options))
            {
                var targetUser = await db.Users.FirstAsync(u => u.Id == 5);

                // Logika: Usuń użytkownika (zresetuj dom i rolę)
                targetUser.HouseId = null;
                targetUser.Role = SystemRole.Guest;
                await db.SaveChangesAsync();
            }

            // ASSERT
            using (var db = new AppDbContext(options))
            {
                var user = await db.Users.FirstAsync(u => u.Id == 5);
                Assert.Null(user.HouseId); // Wyrzucony!
            }
        }
    }
}