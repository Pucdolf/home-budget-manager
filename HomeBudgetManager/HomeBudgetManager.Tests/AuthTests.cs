using Xunit;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HomeBudgetManager.Tests
{
    public class AuthTests
    {
        // 1. TEST REJESTRACJI: Czy RegisterService dodaje usera do bazy?
        [Fact]
        public void RegisterUser_ShouldSaveUserToDatabase()
        {
            // ARRANGE - Baza w pamięci
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthTest_Reg_" + Guid.NewGuid())
                .Options;

            using (var db = new AppDbContext(options))
            {
                // Tworzymy serwis rejestracji (zakładam, że przyjmuje DB w konstruktorze)
                var registerService = new RegisterService(db);

                // ACT - Rejestrujemy
                registerService.RegisterUser("nowy@test.pl", "NowyUser", "Haslo123");
            }

            // ASSERT - Sprawdzamy w nowym kontekście czy user istnieje
            using (var db = new AppDbContext(options))
            {
                var user = db.Users.FirstOrDefault(u => u.Email == "nowy@test.pl");

                Assert.NotNull(user); // Musi istnieć
                Assert.Equal("NowyUser", user.Login);
                Assert.NotEqual("Haslo123", user.Password); // Hasło MUSI być zahashowane (inne niż plain text)
            }
        }

        //// 2. TEST LOGOWANIA (SUKCES): Dobre dane -> True
        //[Fact]
        //public void ValidateUser_ShouldReturnTrue_ForCorrectCredentials()
        //{
        //    var options = new DbContextOptionsBuilder<AppDbContext>()
        //        .UseInMemoryDatabase(databaseName: "AuthTest_Login_" + Guid.NewGuid())
        //        .Options;

        //    // Najpierw musimy kogoś zarejestrować, żeby mieć pewność, że hash hasła jest zgodny z logiką systemu
        //    using (var db = new AppDbContext(options))
        //    {
        //        var registerService = new RegisterService(db);
        //        registerService.RegisterUser("admin@test.pl", "Admin", "Tajne123");
        //    }

        //    // Teraz próbujemy się zalogować
        //    using (var db = new AppDbContext(options))
        //    {
        //        var authService = new AuthService(db);
        //        bool result = authService.ValidateUser("admin@test.pl", "Tajne123");

        //        Assert.True(result, "Logowanie powinno się udać dla poprawnego hasła!");
        //    }
        //}

        // 3. TEST LOGOWANIA (BŁĄD): Złe hasło -> False
        [Fact]
        public void ValidateUser_ShouldReturnFalse_ForWrongPassword()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "AuthTest_LoginFail_" + Guid.NewGuid())
                .Options;

            using (var db = new AppDbContext(options))
            {
                var registerService = new RegisterService(db);
                registerService.RegisterUser("user@test.pl", "User", "DobreHaslo");
            }

            using (var db = new AppDbContext(options))
            {
                var authService = new AuthService(db);
                // Próbujemy zalogować się złym hasłem
                bool result = authService.ValidateUserByEmail("user@test.pl", "ZleHaslo");

                Assert.False(result, "Logowanie powinno zostać odrzucone!");
            }
        }
    }
}