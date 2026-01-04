using HomeBudgetManager.Core.DBTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetManager.Core
{
    internal class User
    {
        private int id;
        private String username;
        private String email;
        private String password;
        private List<Transaction> transactions;
        private List<Category> categories;
        private int? houseId;

        public User()
        {
            this.id = 0;
            this.username = "guest";
            this.transactions = new List<Transaction>();
            this.categories = new List<Category>();
        }

        public User(String username)
        {
            // Add id generator
            this.id = 0;
            this.username = username;
            this.transactions = new List<Transaction>();
            // Add default categories
            this.categories = new List<Category>();
        }

        public User(String username, List<Transaction> transactions, List<Category> categories)
        {
            this.id = 0;
            this.username = username;
            this.transactions = transactions;
            this.categories = categories;
        }

        public User(int id, string username, List<Transaction> transactions, List<Category> categories)
        {
            this.id = id;
            this.username = username;
            this.transactions = transactions;
            this.categories = categories;
        }

        // Manage transactions
        public List<Transaction> GetTransactions() { return transactions; }
        public void addTransaction(Transaction newTransaction)
        {
            this.transactions.Add(newTransaction);
        }

        public async Task removeTransaction(int transactionId)
        {
            //AppDbContext db;
            //var transaction = await db.Transactions.FindAsync(transactionId);

            //if (transaction != null)
            //{
            //    db.Transactions.Remove(transaction);
            //}
        }

        public List<Transaction> getTransactionsByCategory(Category category)
        {
            return new List<Transaction>();
        }

        public List<Transaction> getTransactionsByDate(DateTime start, DateTime end)
        {
            return new List<Transaction>();
        }

        // Manage categories
        public void addCategory(Category newCategory)
        {
            this.categories.Add(newCategory);
        }

        public async Task removeCategory(int id)
        { 
        }

        public List<Category> GetCategories() { return this.categories; }
    }
}
