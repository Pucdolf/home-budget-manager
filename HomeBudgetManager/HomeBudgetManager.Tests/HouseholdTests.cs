using Xunit;
using HomeBudgetManager.Core.DBTables; 

namespace HomeBudgetManager.Tests
{
    public class HouseholdTests
    {

        [Fact]
        public void CreateHousehold_ShouldReturnSuccess_WhenNameIsValidAndUserHasNoHouse()
        {
            // Testujemy scenariusz idealny:
            // User bez domu -> Tworzy dom -> Sukces
            throw new System.NotImplementedException();
        }

        [Fact]
        public void CreateHousehold_ShouldFail_WhenNameIsEmpty()
        {
            // Zgodnie z CreateHouseholdEndpoint.cs:
            // if (string.IsNullOrWhiteSpace(name)) -> Błąd
            throw new System.NotImplementedException();
        }

        [Fact]
        public void CreateHousehold_ShouldFail_WhenUserAlreadyHasHouse()
        {
            // Zgodnie z kodem:
            // if (user.HouseId != null) -> Błąd "użytkownik należy już do domostwa"
            throw new System.NotImplementedException();
        }

        // --- TESTY DOŁĄCZANIA (JOIN) ---

        [Fact]
        public void JoinHousehold_ShouldSuccess_WhenCodeIsCorrect()
        {
            // Scenariusz: Podajemy dobry kod (JoinCode) -> User zostaje dodany do DBHouse
            throw new System.NotImplementedException();
        }

        [Fact]
        public void JoinHousehold_ShouldFail_WhenCodeIsInvalid()
        {
            // Scenariusz: Podajemy kod, którego nie ma w bazie -> Błąd
            throw new System.NotImplementedException();
        }

        [Fact]
        public void JoinHousehold_ShouldFail_WhenUserIsAlreadyMember()
        {
            // Scenariusz: Próbujemy dołączyć, ale już mamy HouseId != null
            throw new System.NotImplementedException();
        }
    }
}