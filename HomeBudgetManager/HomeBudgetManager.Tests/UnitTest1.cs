using Xunit;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using System.Collections.Generic;

namespace HomeBudgetManager.Tests
{
    public class CategoryTests
    {

        [Fact]
        public void AddCategory_ShouldSucceed_WhenNameIsUniqueAndValid()
        {
            // Test happy path: User dodaje nową kategorię
            // Arrange: User, nowa nazwa "Kryptowaluty"
            // Act: service.addCategory(...)
            // Assert: Kategoria jest w bazie i ma przypisane UserId
            throw new System.NotImplementedException();
        }

        [Fact]
        public void AddCategory_ShouldFail_WhenNameIsEmpty()
        {
            // Walidacja pustego stringa
            throw new System.NotImplementedException();
        }

        [Fact]
        public void AddCategory_ShouldFail_WhenNameAlreadyExistsForUser()
        {
            // Test na duplikaty: User ma już "Wakacje", próbuje dodać "Wakacje" ponownie
            throw new System.NotImplementedException();
        }

        [Fact]
        public void AddCategory_ShouldAssignUserId_ToNewCategory()
        {
            // Test na potencjalny BUG w kodzie:
            // Sprawdzam, czy nowa kategoria ma UserId != null.
            // Obecny kod service.addCategory tego nie robi, więc ten test 
            // będzie (słusznie) failował lub wykryje błąd przy implementacji.
            throw new System.NotImplementedException();
        }

        [Fact]
        public void ListCategories_ShouldReturnSystemAndUserCategories()
        {
            // Sprawdzamy czy metoda listAllUserCategories zwraca sumę:
            // kategorie systemowe (UserId == null) + kategorie usera (UserId == X)
            throw new System.NotImplementedException();
        }


        [Fact]
        public void Endpoint_List_ShouldContainAddOption()
        {
            // Sprawdzenie czy wygenerowany HTML zawiera <option value='new-category'>
            throw new System.NotImplementedException();
        }
    }
}