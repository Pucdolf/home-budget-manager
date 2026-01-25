using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace HomeBudgetManager.Core
{
    public class ReportService
    {
        private readonly AppDbContext _db;

        public ReportService(AppDbContext db)
        {
            _db = db;
        }

        public byte[] GeneratePdfReport(int requestingUserId, DateTime startDate, DateTime endDate)
        {
            // 1. Get scope (Household)
            var requestingUser = _db.Users.FirstOrDefault(u => u.Id == requestingUserId);
            if (requestingUser == null) return Array.Empty<byte>();

            List<int> userIds = new List<int>();
            string scopeTitle = "Raport Indywidualny";

            if (requestingUser.HouseId.HasValue)
            {
                userIds = _db.Users.Where(u => u.HouseId == requestingUser.HouseId).Select(u => u.Id).ToList();
                scopeTitle = "Raport Domostwa";
            }
            else
            {
                userIds.Add(requestingUserId);
            }

            // 2. Fetch Data
            var usersData = _db.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new
                {
                    User = u,
                    Transactions = _db.Transactions
                        .Include(t => t.Category)
                        .Where(t => t.UserId == u.Id && t.Date >= startDate && t.Date <= endDate)
                        .OrderBy(t => t.Date)
                        .ToList()
                })
                .ToList();

            // 3. Generate PDF
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
                                column.Item().PageBreak(); // New page for each user or significant section
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
            // ZMIANA: Tworzymy kulturę polską, aby wymusić formatowanie w PLN (zł)
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

                // ZMIANA: Używamy .ToString("C2", pl) zamiast interpolacji $"{var:C2}"
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
                        
                        barCol.Item().Height((float)(150 * (1 - ratio))); // Empty space
                        barCol.Item().Height((float)(150 * ratio)).Background(Colors.Green.Lighten2).Border(1).BorderColor(Colors.Green.Darken2); // Bar
                    });
                    // ZMIANA: Formatowanie C0 (waluta bez groszy) z polską kulturą
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

                        barCol.Item().Height((float)(150 * (1 - ratio))); // Empty space
                        barCol.Item().Height((float)(150 * ratio)).Background(Colors.Red.Lighten2).Border(1).BorderColor(Colors.Red.Darken2); // Bar
                    });
                    // ZMIANA: Formatowanie C0 z polską kulturą
                    c.Item().Text(expense.ToString("C0", pl)).AlignCenter().FontSize(9);
                });
                
                row.RelativeItem(2); // Spacer
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
                    
                    // ZMIANA: Formatowanie C2 z polską kulturą
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
    }
}