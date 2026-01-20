using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Globalization;

namespace HomeBudgetManager.Core
{
    public class ChartService
    {
        private readonly AppDbContext _db;

        public ChartService(AppDbContext db)
        {
            _db = db;
        }

        public class CategoryStat
        {
            public required string CategoryName { get; set; }
            public decimal TotalAmount { get; set; }
            public double Percentage { get; set; }
            public string Color { get; set; } = "#ccc";
        }

        public (List<CategoryStat> Expenses, List<CategoryStat> Incomes) GetStatistics(int userId, DateTime startDate, DateTime endDate)
        {
            var transactions = _db.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
                .ToList();

            var expenses = transactions
                .Where(t => t.TransactionType == TransactionType.expense)
                .GroupBy(t => t.Category?.Name ?? "Brak kategorii")
                .Select(g => new { Name = g.Key, Total = g.Sum(t => Math.Abs(t.Value)) })
                .ToList();

            var incomes = transactions
                .Where(t => t.TransactionType == TransactionType.income)
                .GroupBy(t => t.Category?.Name ?? "Brak kategorii")
                .Select(g => new { Name = g.Key, Total = g.Sum(t => Math.Abs(t.Value)) })
                .ToList();

            var totalExpense = expenses.Sum(x => x.Total);
            var totalIncome = incomes.Sum(x => x.Total);

            var expenseStats = expenses.Select(x => new CategoryStat
            {
                CategoryName = x.Name,
                TotalAmount = x.Total,
                Percentage = totalExpense == 0 ? 0 : (double)(x.Total / totalExpense) * 100
            }).OrderByDescending(x => x.Percentage).ToList();

            var incomeStats = incomes.Select(x => new CategoryStat
            {
                CategoryName = x.Name,
                TotalAmount = x.Total,
                Percentage = totalIncome == 0 ? 0 : (double)(x.Total / totalIncome) * 100
            }).OrderByDescending(x => x.Percentage).ToList();

            // Simple color palette
            string[] colors = [
                "#124708", // green
                "#d66d24", // orange
                "#efc766", // yellow-beige
                "#005f73", // teal
                "#ae2012", // red
                "#94d2bd", // light teal
                "#e9d8a6", // light beige
                "#6b705c"  // olive
            ];

            for (int i = 0; i < expenseStats.Count; i++) expenseStats[i].Color = colors[i % colors.Length];
            for (int i = 0; i < incomeStats.Count; i++) incomeStats[i].Color = colors[i % colors.Length];

            return (expenseStats, incomeStats);
        }

        public string GenerateChartsHtml(int userId, DateTime startDate, DateTime endDate)
        {
            var (expenses, incomes) = GetStatistics(userId, startDate, endDate);
            var sb = new StringBuilder();

            // Using flexbox for side-by-side or stacked layout
            sb.Append("<div class='charts-container' style='display: flex; flex-wrap: wrap; gap: 20px; justify-content: center; margin-top: 20px;'>");

            sb.Append(GenerateSingleChartHtml("Wydatki", expenses));
            sb.Append(GenerateSingleChartHtml("Przychody", incomes));

            sb.Append("</div>");
            return sb.ToString();
        }

        private string GenerateSingleChartHtml(string title, List<CategoryStat> stats)
        {
            if (stats.Count == 0)
            {
                return $@"
                    <div class='chart-box' style='flex: 1; min-width: 300px; max-width: 500px; text-align: center; border: 1px solid #e0e0e0; padding: 20px; border-radius: 12px; background-color: #fff;'>
                        <h3 style='margin-top: 0; color: #333;'>{title}</h3>
                        <p style='color: #666;'>Brak danych w wybranym okresie.</p>
                    </div>";
            }

            var sb = new StringBuilder();
            sb.Append($@"
            <div class='chart-box' style='flex: 1; min-width: 300px; max-width: 500px; border: 1px solid #e0e0e0; padding: 20px; border-radius: 12px; background-color: #fff; box-shadow: 0 2px 4px rgba(0,0,0,0.05);'>
                <h3 style='text-align: center; margin-top: 0; margin-bottom: 20px; color: #333;'>{title}</h3>
                <div class='pie-chart-wrapper' style='display: flex; align-items: center; justify-content: center; flex-wrap: wrap; gap: 20px;'>
            ");

            // Generate Conic Gradient
            // conic-gradient(color1 0% 20%, color2 20% 50%, ...)
            var gradientParts = new List<string>();
            double currentPercent = 0;
            foreach (var stat in stats)
            {
                double nextPercent = currentPercent + stat.Percentage;
                // Using InvariantCulture to ensure dot separator for decimals in CSS
                gradientParts.Add($"{stat.Color} {currentPercent.ToString(CultureInfo.InvariantCulture)}% {nextPercent.ToString(CultureInfo.InvariantCulture)}%");
                currentPercent = nextPercent;
            }
            // Fallback if parts empty (shouldn't happen due to check above)
            string gradientStyle = $"background: conic-gradient({string.Join(", ", gradientParts)});";

            sb.Append($@"
                    <div class='pie-chart' style='width: 180px; height: 180px; border-radius: 50%; {gradientStyle}'></div>
                    <ul class='chart-legend' style='list-style: none; padding: 0; margin: 0; font-size: 0.9em; max-width: 200px;'>
            ");

            var culture = new CultureInfo("pl-PL");
            foreach (var stat in stats)
            {
                sb.Append($@"
                        <li style='margin-bottom: 8px; display: flex; align-items: center; color: #555;'>
                            <span style='display: inline-block; width: 14px; height: 14px; background-color: {stat.Color}; margin-right: 10px; border-radius: 3px; flex-shrink: 0;'></span>
                            <span><strong>{stat.CategoryName}</strong>: {stat.Percentage:F1}% <br><span style='font-size: 0.85em; color: #888;'>({stat.TotalAmount.ToString("C2", culture)})</span></span>
                        </li>
                ");
            }

            sb.Append("</ul></div></div>");
            return sb.ToString();
        }
    }
}
