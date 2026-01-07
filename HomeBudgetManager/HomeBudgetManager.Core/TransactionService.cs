using HomeBudgetManager.Core.DBTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetManager.Core
{
    public class TransactionService
    {
        AppDbContext db;

        public TransactionService(AppDbContext db)
        {
            this.db = db;
        }

        public string addTransaction(DBTransaction transaction)
        {
            try
            {
                db.Add(transaction);
                db.SaveChanges();
                return "Pomyślnie dodano transakcję";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string addTransaction(int userId, int categoryId, decimal value, bool isRepeatable, string? description, int? houseId)
        {
            var newTransaction = new DBTransaction { UserId = userId, CategoryId = categoryId, Value = value, IsRepeatable = isRepeatable, Description = description, HouseId = houseId };
            db.Add(newTransaction);
            db.SaveChanges();
            return "Pomyślnie dodano transakcję";
        }

        public string editTransaction(int transactionId, int categoryId, decimal value, bool isRepeatable, string? description, int? houseId)
        {
            var transaction = db.Transactions.FirstOrDefault(t => t.Id == transactionId);

            if (transaction == null)
            {
                return "Błąd: nie znaleziono transakcji";
            }

            if (description == null)
            {
                description = transaction.Description;
            }

            if (houseId == null)
            {
                houseId = transaction.HouseId;
            }

            transaction = new DBTransaction { Id = transactionId, UserId = transaction.UserId, CategoryId = categoryId, Value = value, IsRepeatable = isRepeatable, Description = description, HouseId = houseId };
            db.SaveChanges();
            return "Pomyślnie zedytowano transakcję";
        }

        public string deleteTransaction(int transactionId, int userId)
        {
            var transaction = db.Transactions.FirstOrDefault(t => t.Id == transactionId && t.UserId == userId);

            if (transaction == null)
            {
                return "Błąd: Użytkownik nie posiada takiej transakcji";
            }

            db.Remove(transaction);
            db.SaveChanges();
            return "Pomyślnie usunięto transakcję";
        }

        public StringBuilder listTransactionsForHtml(List<DBTransaction> transactions)
        {
            var sb = new System.Text.StringBuilder();

            foreach (var t in transactions)
            {
                string date = t.Date.ToString("dd.MM.yyyy");
                string amount = t.Value.ToString("C2", new System.Globalization.CultureInfo("pl-PL"));
                string colorClass = t.Value < 0 ? "amount-expense" : "amount-income";

                sb.Append($"""

                    <li class="transaction-item">
                        <div class="transaction-amount {colorClass}">
                            {amount}
                        </div>
                        <div class="transaction-details">
                            <span class="category-badge">{t.Category}</span>
                            <span class="transaction-date">{date}</span>
                        </div>
                    </li>

                 """);
            }

            return sb;
        }

        public List<DBTransaction> AllUserTransactions(int userId)
        {
            return db.Transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.Date).ToList();
        }

        public List<DBTransaction> SomeUserTransactions(int userId, int amount)
        {
            return db.Transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.Date).Take(amount).ToList();
        }

        public List<DBTransaction> allHouseTransactions(int houseId)
        {
            return db.Transactions.Where(t => t.HouseId == houseId).OrderByDescending(t => t.Date).ToList();
        }

        public List<DBTransaction> someHouseTransactions(int houseId, int amount)
        {
            return db.Transactions.Where(t => t.HouseId == houseId).OrderByDescending(t => t.Date).ToList();
        }
    }
}
