using Xunit;
using HomeBudgetManager.Core;

namespace HomeBudgetManager.Tests
{
    public class AuthTests
    {
        // Te testy będą sprawdzać logikę rejestracji i logowania.
        // Na razie to "stuby" (puste testy).

        [Fact]
        public void Register_ShouldReturnSuccess_WhenDataIsCorrect()
        {
            // TODO: Zaimplementuj test poprawnej rejestracji
            // Arrange: Przygotuj obiekt User z poprawnymi danymi
            // Act: Wywołaj RegisterService.Register(...)
            // Assert: Sprawdź czy wynik to sukces
            throw new System.NotImplementedException();
        }

        [Fact]
        public void Register_ShouldFail_WhenUserAlreadyExists()
        {
            // TODO: Zaimplementuj test próby rejestracji na zajęty login/email
            throw new System.NotImplementedException();
        }

        [Fact]
        public void Register_ShouldFail_WhenPasswordIsEmpty()
        {
            // TODO: Zaimplementuj test walidacji hasła
            throw new System.NotImplementedException();
        }

        [Fact]
        public void Login_ShouldReturnToken_WhenCredentialsAreCorrect()
        {
            // TODO: Zaimplementuj test poprawnego logowania (AuthService)
            throw new System.NotImplementedException();
        }

        [Fact]
        public void Login_ShouldFail_WhenPasswordIsIncorrect()
        {
            // TODO: Zaimplementuj test logowania ze złym hasłem
            throw new System.NotImplementedException();
        }
    }
}