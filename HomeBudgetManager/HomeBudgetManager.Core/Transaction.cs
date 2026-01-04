using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetManager.Core
{

    enum TransactionType
    {
        Expense,
        Income
    }

    internal class Transaction
    {
        private int id;
        private float amount;
        private DateTime date;
        private Category category;
        private TransactionType transactionType;
        private String? description;
        private int? userId;
        private int? houseId;
        private bool isRepetetive = false;

        public Transaction(float amount, Category category, TransactionType transactionType, String? description)
        {
            this.amount = amount;
            this.date = DateTime.Now;
            this.category = category;
            this.transactionType = transactionType;
            this.description = description;
        }

        public Transaction(int id, float amount, Category category, TransactionType transactionType, String? description)
        {
            this.id = id;
            this.amount = amount;
            this.category = category;
            this.date = DateTime.Now;
            this.transactionType = transactionType;
            this.description = description;
        }

        public int getId() { return id; }

        public float getAmount() { return amount; }
        
    }
}
