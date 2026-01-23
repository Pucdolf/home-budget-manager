using Xunit;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;

namespace HomeBudgetManager.Tests
{
    public class TransactionTests
    {

        [Fact]
        public void AddTransaction_ShouldSucceed_WhenDataIsValid()
        {
            // Test poprawnego dodania zwykłego przychodu
            // Arrange: Przygotuj Usera, Kategorię, Kwotę
            // Act: Wywołaj TransactionService.addTransaction
            // Assert: Sprawdź czy transakcja jest w bazie
            throw new System.NotImplementedException();
        }

        [Fact]
        public void AddTransaction_ShouldSaveNegativeValue_WhenTypeIsExpense()
        {
            // WAŻNE: Endpoint zamienia kwotę na ujemną: amount = (type == 0) ? -amount : amount;
            // Musimy sprawdzić, czy jak podam "50" i typ "Expense", to w bazie zapisze się "-50"
            throw new System.NotImplementedException();
        }

        [Fact]
        public void AddTransaction_ShouldCreateRecurringEntry_WhenIsRecurringIsTrue()
        {
            // Jeśli flaga isRecurring = true, TransactionService powinien dodać wpis
            // nie tylko do tabeli Transactions, ale też DBRepetableTransaction
            throw new System.NotImplementedException();
        }


        [Fact]
        public void AddTransaction_ShouldFail_WhenAmountFormatIsInvalid()
        {
            // Symulacja wysłania stringa "abc" zamiast liczby
            // Oczekujemy błędu lub komunikatu HTML "Błędny format kwoty"
            throw new System.NotImplementedException();
        }

        [Fact]
        public void AddTransaction_ShouldFail_WhenUserDoesNotExist()
        {
            // Próba dodania transakcji dla UserID, którego nie ma w bazie
            throw new System.NotImplementedException();
        }
    }
}