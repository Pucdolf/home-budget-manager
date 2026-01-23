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

        public void addTransaction(DBTransaction transaction)
        {
            try
            {
                db.Add(transaction);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("<div clas='error'>" + ex.ToString() + "</div>");
            }
        }

        public void addTransaction(int userId, int categoryId, decimal value, TransactionType type, DateTime date, bool isRepeatable, decimal? interval, string? description,  int? houseId)
        {
            var newTransaction = new DBTransaction { UserId = userId, CategoryId = categoryId, Value = value, TransactionType = type, Date = date, IsRepeatable = isRepeatable, Description = description, HouseId = houseId };
            db.Add(newTransaction);
            db.SaveChanges();

            if (isRepeatable && interval != null)
            {
                try
                {
                    var newRepTransaction = new DBRepetableTransaction { TransactionId = newTransaction.Id, TransactionInterval = (decimal)interval };
                    db.Add(newRepTransaction);
                    db.SaveChanges();
                } catch
                {
                    throw new InvalidOperationException("<div class='error'>Błąd: nie dodano transakcji okresowej</div>");
                }
            }
        }

        public void editTransaction(int transactionId, int categoryId, decimal value, bool isRepeatable, string? description, int? houseId)
        {
            var transaction = db.Transactions.FirstOrDefault(t => t.Id == transactionId);

            if (transaction == null)
            {
                throw new ArgumentNullException("<div class='error'>Błąd: nie znaleziono transakcji po ID</div>");
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
        }

        public void deleteTransaction(int transactionId, int userId)
        {
            var transaction = db.Transactions.FirstOrDefault(t => t.Id == transactionId && t.UserId == userId);

            if (transaction == null)
            {
                throw new ArgumentNullException("<div class='error'>Błąd: nie znaleziono transakcji po ID</div>");
            }

            try
            {
                db.Remove(transaction);
                db.SaveChanges();
            } catch (Exception ex)
            {
                throw new InvalidOperationException("<div class='error>" + ex.Message + "</div>");
            }
        }

        public StringBuilder listTransactionsForDashboard(List<DBTransaction> transactions)
        {
            var sb = new System.Text.StringBuilder();

            foreach (var t in transactions)
            {
                string date = t.Date.ToString("yyyy-MM-dd"); // Use ISO format for JS
                string displayDate = t.Date.ToString("dd.MM.yyyy");
                string amount = t.Value.ToString("C2", new System.Globalization.CultureInfo("pl-PL"));
                string colorClass = t.Value < 0 ? "amount-expense" : "amount-income";
                var category = db.Categories.FirstOrDefault(c => c.Id == t.CategoryId);
                
                string safeDescription = (t.Description ?? "").Replace("\"", "&quot;").Replace("'", "\\'");

                sb.Append($"""

                    <li class="transaction-item" onclick="openDashboardTransactionDetails({t.Id}, '{t.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}', '{safeDescription}', '{date}')" style="cursor: pointer;">
                        <div class="transaction-amount {colorClass}">
                            {amount}
                        </div>
                        <div class="transaction-details">
                            <span class="category-badge">{category.Name}</span>
                            <span class="transaction-date">{displayDate}</span>
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
            return db.Transactions.Where(t => t.UserId == userId && t.Date <= DateTime.Now).OrderByDescending(t => t.Date).Take(amount).ToList();
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
