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
        private List<DBTransaction> transactions;
        private List<DBCategory> categories;
        private int? houseId;

        public User()
        {
            this.id = 0;
            this.username = "guest";
            this.transactions = new List<DBTransaction>();
            this.categories = new List<DBCategory>();
        }

        public User(String username)
        {
            // Add id generator
            this.id = 0;
            this.username = username;
            this.transactions = new List<DBTransaction>();
            // Add default categories
            this.categories = new List<DBCategory>();
        }

        public User(String username, List<DBTransaction> transactions, List<DBCategory> categories)
        {
            this.id = 0;
            this.username = username;
            this.transactions = transactions;
            this.categories = categories;
        }

        public User(int id, string username, List<DBTransaction> transactions, List<DBCategory> categories)
        {
            this.id = id;
            this.username = username;
            this.transactions = transactions;
            this.categories = categories;
        }
    }
}
