using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Core.Enums;
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

        public void addTransaction(int userId, int categoryId, decimal value, TransactionType type, DateTime date, bool isRepeatable, int? transactionInterval, string title, string? description,  int? houseId, int? frequencyUnit)
        {
            var newTransaction = new DBTransaction { UserId = userId, CategoryId = categoryId, Value = value, TransactionType = type, Date = date, IsRepeatable = isRepeatable, Title = title, Description = description, HouseId = houseId };
            db.Add(newTransaction);
            db.SaveChanges();

            if (isRepeatable && transactionInterval.HasValue && frequencyUnit.HasValue)
            {
                try
                {
                    DateTime nextRunDate = date;
                    var unit = (TransactionIntervalType)frequencyUnit.Value;

                    nextRunDate = unit switch
                    {
                        TransactionIntervalType.Days => date.AddDays(transactionInterval.Value),
                        TransactionIntervalType.Weeks => date.AddDays(transactionInterval.Value * 7),
                        TransactionIntervalType.Months => date.AddMonths(transactionInterval.Value),
                        TransactionIntervalType.Years => date.AddYears(transactionInterval.Value),
                        _ => date.AddMonths(transactionInterval.Value)
                    };

                    var newRepTransaction = new DBRepetableTransaction
                    {
                        TransactionId = newTransaction.Id,
                        TransactionInterval = transactionInterval.Value,
                        FrequencyUnit = frequencyUnit.Value,
                        Value = value,
                        UserId = userId,
                        CategoryId = categoryId,
                        NextRunDate = nextRunDate, 
                        Title = title, 
                        Description = description 
                    };

                    Console.WriteLine($"DEBUG: Adding Recurring Transaction - Title: '{title}', Amount: {value}, NextRun: {nextRunDate}");

                    db.Add(newRepTransaction);
                    db.SaveChanges();
                }
                catch
                {
                    throw new InvalidOperationException("<div class='error'>Błąd: nie dodano transakcji okresowej</div>");
                }
            }
        }

        public void editTransaction(int transactionId, int categoryId, decimal value, bool isRepeatable, string title, string? description, int? houseId)
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

            transaction.CategoryId = categoryId;
            transaction.Value = value;
            transaction.IsRepeatable = isRepeatable;
            transaction.Title = title;
            transaction.Description = description;
            transaction.HouseId = houseId;
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
                
                string safeDescription = (t.Description ?? "").Replace("\"", "&quot;").Replace("'", "\\'").Replace("\r", "").Replace("\n", " ");
                string safeTitle = (t.Title ?? "Bez tytułu").Replace("\"", "&quot;").Replace("'", "\\'").Replace("\r", "").Replace("\n", " ");

                sb.Append($"""

                    <li class="transaction-item" onclick="openDashboardTransactionDetails({t.Id}, '{t.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}', '{safeTitle}', '{safeDescription}', '{date}')" style="cursor: pointer;">
                        <div class="transaction-info">
                             <div class="transaction-title">{safeTitle}</div>
                             <div class="transaction-details-sub">
                                <span class="category-badge">{category?.Name ?? "Brak"}</span>
                                <span class="transaction-date">{displayDate}</span>
                             </div>
                        </div>
                        <div class="transaction-amount {colorClass}">
                            {amount}
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
