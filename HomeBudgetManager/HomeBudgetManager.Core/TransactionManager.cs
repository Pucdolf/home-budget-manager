using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetManager.Core
{
    internal class TransactionManager
    {
        public static Transaction createTransaction(float amount, String description, Category category, TransactionType type)
        {
            return new Transaction(100, amount, category, type, description);
        }

        // additional checks of transaction
        public static void validateTransaction(Transaction transaction)
        {

        }

        // count a total sum of a user's transactions
        public static float calculateTotal(List<Transaction> transactions)
        {
            
            float total = 0;
            foreach (Transaction transaction in transactions)
            {
                float temp = transaction.getAmount();
                total += temp;
            }

            return total;
        }
    }
}
