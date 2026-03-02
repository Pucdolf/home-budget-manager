using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using HomeBudgetManager.Core.Enums;

namespace HomeBudgetManager.Core
{
    public class ReportService
    {
        private readonly AppDbContext _db;

        public ReportService(AppDbContext db)
        {
            _db = db;
        }

        public byte[] GeneratePdfReport(int requestingUserId, DateTime startDate, DateTime endDate, bool includeHousehold)
        {
            // 1. Get scope
            var requestingUser = _db.Users.FirstOrDefault(u => u.Id == requestingUserId);
            if (requestingUser == null) return Array.Empty<byte>();

            List<int> userIds = new List<int>();
            string scopeTitle = "Raport Indywidualny";

            if (includeHousehold && requestingUser.HouseId.HasValue)
            {
                userIds = _db.Users.Where(u => u.HouseId == requestingUser.HouseId).Select(u => u.Id).ToList();
                scopeTitle = "Raport Domostwa";
            }
            else
            {
                userIds.Add(requestingUserId);
            }

            // 2. Fetch Real Transactions
            var realTransactions = _db.Transactions
                .Include(t => t.Category)
                .Where(t => userIds.Contains(t.UserId) && t.Date >= startDate && t.Date <= endDate)
                .ToList();

            // 3. Fetch Recurring Rules & Project Virtual Transactions
            var recurringRules = _db.RepetableTransactions
                .Include(rt => rt.Category)
                .Where(rt => userIds.Contains(rt.UserId) && rt.IsActive)
                .ToList();

            var virtualTransactions = new List<DBTransaction>();

            foreach (var rule in recurringRules)
            {
                var iterDate = rule.NextRunDate;
                
                if (rule.TransactionInterval <= 0) continue;

                while (iterDate <= endDate)
                {
                    if (iterDate >= startDate)
                    {
                        virtualTransactions.Add(new DBTransaction
                        {
                            Id = 0, // Virtual
                            UserId = rule.UserId,
                            CategoryId = rule.CategoryId,
                            Category = rule.Category,
                            Value = rule.Value,
                            Title = rule.Title + " (Plan)",
                            Description = rule.Description,
                            Date = iterDate,
                            TransactionType = (rule.Value < 0) ? TransactionType.expense : TransactionType.income
                        });
                    }
                    iterDate = CalculateNextDate(iterDate, rule.TransactionInterval, rule.FrequencyUnit);
                }
            }

            var allTransactions = realTransactions.Concat(virtualTransactions).OrderBy(t => t.Date).ToList();

            // 4. Group data for PDF
            var usersData = userIds.Select(uid => new
            {
                User = _db.Users.FirstOrDefault(u => u.Id == uid),
                Transactions = allTransactions.Where(t => t.UserId == uid).ToList()
            })
            .Where(d => d.User != null)
            .ToList();

            // 5. Generate PDF
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .Text(text =>
                        {
                            text.Span("Home Budget Manager").SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);
                            text.Span($" - {scopeTitle}").FontSize(16).FontColor(Colors.Grey.Medium);
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Item().Text($"Okres: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}").FontSize(12).SemiBold();
                            column.Item().PaddingBottom(10);

                            foreach (var data in usersData)
                            {
                                GenerateUserSection(column, data.User.Login, data.Transactions);
                                column.Item().PageBreak(); 
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Strona ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }

        private void GenerateUserSection(ColumnDescriptor column, string username, List<DBTransaction> transactions)
        {
            // Ustawiamy kulturę polską
            var pl = new CultureInfo("pl-PL");

            var income = transactions.Where(t => t.Value > 0).Sum(t => t.Value);
            var expense = Math.Abs(transactions.Where(t => t.Value < 0).Sum(t => t.Value));
            var balance = income - expense;

            column.Item().Text($"Użytkownik: {username}").FontSize(16).Bold().FontColor(Colors.Black);
            column.Item().PaddingBottom(5);

            // Summary Table
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Przychody").FontColor(Colors.Green.Medium).SemiBold();
                    header.Cell().Element(CellStyle).Text("Wydatki").FontColor(Colors.Red.Medium).SemiBold();
                    header.Cell().Element(CellStyle).Text("Bilans").SemiBold();
                });

                table.Cell().Element(CellStyle).Text(income.ToString("C2", pl));
                table.Cell().Element(CellStyle).Text(expense.ToString("C2", pl));
                table.Cell().Element(CellStyle).Text(balance.ToString("C2", pl));
            });

            column.Item().PaddingBottom(20);

            // Bar Chart Simulation
            column.Item().Text("Wizualizacja").FontSize(14).SemiBold();
            column.Item().PaddingBottom(10);
            
            var maxValue = Math.Max(income, expense);
            if (maxValue == 0) maxValue = 1;

            column.Item().Row(row =>
            {
                // Income Bar
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Przychód").AlignCenter().FontSize(10);
                    c.Item().Height(150).Column(barCol => 
                    {
                        var ratio = (double)income / (double)maxValue;
                        if (ratio < 0) ratio = 0;
                        if (ratio > 1) ratio = 1;
                        
                        barCol.Item().Height((float)(150 * (1 - ratio))); 
                        barCol.Item().Height((float)(150 * ratio)).Background(Colors.Green.Lighten2).Border(1).BorderColor(Colors.Green.Darken2); 
                    });
                    c.Item().Text(income.ToString("C0", pl)).AlignCenter().FontSize(9);
                });

                row.Spacing(20);

                // Expense Bar
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Wydatek").AlignCenter().FontSize(10);
                    c.Item().Height(150).Column(barCol =>
                    {
                        var ratio = (double)expense / (double)maxValue;
                        if (ratio < 0) ratio = 0;
                        if (ratio > 1) ratio = 1;

                        barCol.Item().Height((float)(150 * (1 - ratio))); 
                        barCol.Item().Height((float)(150 * ratio)).Background(Colors.Red.Lighten2).Border(1).BorderColor(Colors.Red.Darken2); 
                    });
                    c.Item().Text(expense.ToString("C0", pl)).AlignCenter().FontSize(9);
                });
                
                row.RelativeItem(2); 
            });

            column.Item().PaddingBottom(20);

            // Transactions Table
            column.Item().Text("Szczegóły transakcji").FontSize(14).SemiBold();
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(80); // Date
                    columns.RelativeColumn();   // Category
                    columns.RelativeColumn(2);  // Desc
                    columns.ConstantColumn(80); // Amount
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderStyle).Text("Data");
                    header.Cell().Element(HeaderStyle).Text("Kategoria");
                    header.Cell().Element(HeaderStyle).Text("Opis");
                    header.Cell().Element(HeaderStyle).Text("Kwota").AlignRight();
                });

                foreach (var transaction in transactions)
                {
                    table.Cell().Element(CellStyle).Text($"{transaction.Date:dd.MM.yyyy}");
                    table.Cell().Element(CellStyle).Text(transaction.Category?.Name ?? "-");
                    table.Cell().Element(CellStyle).Text(transaction.Description ?? "");
                    
                    var color = transaction.Value < 0 ? Colors.Red.Medium : Colors.Green.Medium;
                    
                    table.Cell().Element(CellStyle).Text(transaction.Value.ToString("C2", pl)).FontColor(color).AlignRight();
                }
            });
        }

        static IContainer CellStyle(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
        }

        static IContainer HeaderStyle(IContainer container)
        {
            return container.BorderBottom(1).BorderColor(Colors.Grey.Medium).PaddingVertical(5).DefaultTextStyle(x => x.SemiBold());
        }

        private DateTime CalculateNextDate(DateTime current, int value, int unit)
        {
            var type = (TransactionIntervalType)unit;
            return type switch
            {
                TransactionIntervalType.Days => current.AddDays(value),
                TransactionIntervalType.Weeks => current.AddDays(value * 7),
                TransactionIntervalType.Months => current.AddMonths(value),
                TransactionIntervalType.Years => current.AddYears(value),
                _ => current.AddMonths(value)
            };
        }
    }
}