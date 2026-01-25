using Xunit;
using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Core;

namespace HomeBudgetManager.Tests
{
    public class HouseholdActionsTests
    {


        [Fact]
        public void LeaveHousehold_ShouldRemoveUser_WhenMemberLeaves()
        {
            // Scenariusz: Zwykły user wychodzi -> Dom zostaje, user ma HouseId = null
            throw new System.NotImplementedException();
        }

        [Fact]
        public void LeaveHousehold_ShouldDeleteHouse_WhenAdminLeaves()
        {
            // Scenariusz: Admin wychodzi -> Dom jest usuwany (db.Houses.Remove)
            // Ważne: Sprawdzić czy inni członkowie też zostali uwolnieni
            throw new System.NotImplementedException();
        }

        [Fact]
        public void LeaveHousehold_ShouldFail_WhenUserHasNoHouse()
        {
            // Próba wywołania endpointu przez kogoś, kto nie ma domu
            throw new System.NotImplementedException();
        }



        [Fact]
        public void RemoveMember_ShouldSucceed_WhenAdminRemovesMember()
        {
            // Scenariusz: Admin wyrzuca Usera B -> User B ma HouseId = null
            throw new System.NotImplementedException();
        }

        [Fact]
        public void RemoveMember_ShouldFail_WhenNonAdminTriesToRemove()
        {
            // Bezpieczeństwo: Zwykły user próbuje wywołać ten endpoint
            throw new System.NotImplementedException();
        }

        [Fact]
        public void RemoveMember_ShouldFail_WhenTargetIsNotInSameHouse()
        {
            // Próba usunięcia kogoś z innego domu (ID usera spoza grupy)
            throw new System.NotImplementedException();
        }
    }
}