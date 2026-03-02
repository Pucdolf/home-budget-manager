using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Core.Enums;
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

        public (List<CategoryStat> Expenses, List<CategoryStat> Incomes) GetStatistics(List<int> userIds, DateTime startDate, DateTime endDate)
        {
            // 1. Fetch Real Transactions
            var transactions = _db.Transactions
                .Include(t => t.Category)
                .Where(t => userIds.Contains(t.UserId) && t.Date >= startDate && t.Date <= endDate)
                .ToList();

            // 2. Fetch Active Recurring Rules
            var recurringRules = _db.RepetableTransactions
                .Include(rt => rt.Category)
                .Where(rt => userIds.Contains(rt.UserId) && rt.IsActive)
                .ToList();

            // 3. Project Future Transactions
            foreach (var rule in recurringRules)
            {
                var currentDate = rule.NextRunDate;
                var unit = (TransactionIntervalType)rule.FrequencyUnit;

                // Loop to find all occurrences within the requested range
                while (currentDate <= endDate)
                {
                    // Only include if it falls within the start-end range
                    if (currentDate >= startDate)
                    {
                        // Create a transient transaction object for calculation
                        var projected = new DBTransaction
                        {
                            // Required fields for DBTransaction (though not saved to DB)
                            Id = 0, // transient
                            UserId = rule.UserId,
                            CategoryId = rule.CategoryId,
                            Category = rule.Category, // Important for grouping
                            Value = rule.Value,
                            Title = "Projected", // Dummy
                            TransactionType = (rule.Value < 0) ? TransactionType.expense : TransactionType.income,
                            Date = currentDate,
                            IsRepeatable = false
                        };

                        transactions.Add(projected);
                    }

                    // Advance to next occurrence
                    currentDate = unit switch
                    {
                        TransactionIntervalType.Days => currentDate.AddDays(rule.TransactionInterval),
                        TransactionIntervalType.Weeks => currentDate.AddDays(rule.TransactionInterval * 7),
                        TransactionIntervalType.Months => currentDate.AddMonths(rule.TransactionInterval),
                        TransactionIntervalType.Years => currentDate.AddYears(rule.TransactionInterval),
                        _ => currentDate.AddMonths(1)
                    };
                }
            }

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

        public string GenerateChartsHtml(int userId, DateTime startDate, DateTime endDate, bool includeHousehold = false)
        {
            List<int> userIds = new List<int> { userId };
            
            if (includeHousehold)
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null && user.HouseId.HasValue)
                {
                    userIds = _db.Users.Where(u => u.HouseId == user.HouseId).Select(u => u.Id).ToList();
                }
            }

            var (expenses, incomes) = GetStatistics(userIds, startDate, endDate);
            var sb = new StringBuilder();

            // Inject simple tooltip CSS and JS
            sb.Append(GetTooltipAssets());

            sb.Append("<div class='charts-container' style='display: flex; flex-wrap: wrap; gap: 20px; justify-content: center; margin-top: 20px;'>");

            sb.Append(GenerateSingleChartHtml(includeHousehold ? "Wydatki (Domostwo)" : "Wydatki (Moje)", expenses));
            sb.Append(GenerateSingleChartHtml(includeHousehold ? "Przychody (Domostwo)" : "Przychody (Moje)", incomes));

            sb.Append("</div>");
            return sb.ToString();
        }

        public string GenerateDashboardChartsHtml(int userId)
        {
             var now = DateTime.Now;
             var startDate = new DateTime(now.Year, now.Month, 1);
             var endDate = now.Date.AddDays(1).AddTicks(-1);
             return GenerateChartsHtml(userId, startDate, endDate, false);
        }

        private string GetTooltipAssets()
        {
            return """
            <style>
                .chart-tooltip {
                    position: fixed;
                    background: rgba(0, 0, 0, 0.8);
                    color: white;
                    padding: 5px 10px;
                    border-radius: 4px;
                    pointer-events: none;
                    font-size: 0.9em;
                    z-index: 1000;
                    display: none;
                    white-space: nowrap;
                }
                .pie-slice:hover {
                    opacity: 0.8;
                    cursor: pointer;
                }
            </style>
            <script>
                document.addEventListener('mousemove', function(e) {
                    var tooltip = document.getElementById('chart-tooltip');
                    if (!tooltip) return;
                    
                    if (e.target.classList.contains('pie-slice')) {
                        var name = e.target.getAttribute('data-name');
                        var val = e.target.getAttribute('data-value');
                        var pct = e.target.getAttribute('data-percent');
                        
                        tooltip.innerHTML = `<strong>${name}</strong><br>${pct}%<br>${val}`;
                        tooltip.style.display = 'block';
                        tooltip.style.left = (e.clientX + 10) + 'px';
                        tooltip.style.top = (e.clientY + 10) + 'px';
                    } else {
                        tooltip.style.display = 'none';
                    }
                });
            </script>
            <div id="chart-tooltip" class="chart-tooltip"></div>
            """;
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

            // SVG Generation
            double currentAngle = -90; // Start at top
            double radius = 100;
            double centerX = 100;
            double centerY = 100;
            var culture = new CultureInfo("pl-PL");

            sb.Append($"<svg width='200' height='200' viewBox='0 0 200 200' style='transform: rotate(0deg);'>");
            
            // Check if single slice 100%
            if (stats.Count == 1)
            {
                 var stat = stats[0];
                 string amountStr = stat.TotalAmount.ToString("C2", culture);
                 sb.Append($"<circle cx='100' cy='100' r='100' fill='{stat.Color}' class='pie-slice' data-name='{stat.CategoryName}' data-value='{amountStr}' data-percent='100' />");
            }
            else
            {
                foreach (var stat in stats)
                {
                    double sliceAngle = (stat.Percentage / 100.0) * 360.0;
                    double x1 = centerX + radius * Math.Cos(currentAngle * Math.PI / 180.0);
                    double y1 = centerY + radius * Math.Sin(currentAngle * Math.PI / 180.0);
                    
                    double endAngle = currentAngle + sliceAngle;
                    double x2 = centerX + radius * Math.Cos(endAngle * Math.PI / 180.0);
                    double y2 = centerY + radius * Math.Sin(endAngle * Math.PI / 180.0);

                    int largeArcFlag = sliceAngle > 180 ? 1 : 0;
                    
                    // M startX startY A radius radius 0 largeArcFlag 1 endX endY L centerX centerY Z
                    string pathData = $"M {x1.ToString(CultureInfo.InvariantCulture)} {y1.ToString(CultureInfo.InvariantCulture)} A {radius} {radius} 0 {largeArcFlag} 1 {x2.ToString(CultureInfo.InvariantCulture)} {y2.ToString(CultureInfo.InvariantCulture)} L {centerX} {centerY} Z";
                    
                    string amountStr = stat.TotalAmount.ToString("C2", culture);
                    
                    sb.Append($"<path d='{pathData}' fill='{stat.Color}' stroke='white' stroke-width='1' class='pie-slice' data-name='{stat.CategoryName}' data-value='{amountStr}' data-percent='{stat.Percentage:F1}' />");
                    
                    currentAngle += sliceAngle;
                }
            }
            sb.Append("</svg>");


            sb.Append(@"
                    <ul class='chart-legend' style='list-style: none; padding: 0; margin: 0; font-size: 0.9em; max-width: 200px;'>
            ");

            foreach (var stat in stats)
            {
                sb.Append($@"
                        <li style='margin-bottom: 8px; display: flex; align-items: center; color: #555;'>
                            <span style='display: inline-block; width: 14px; height: 14px; background-color: {stat.Color}; margin-right: 10px; border-radius: 3px; flex-shrink: 0;'></span>
                            <span><strong>{stat.CategoryName}</strong>: {stat.Percentage:F1}%</span>
                        </li>
                ");
            }

            sb.Append("</ul></div></div>");
            return sb.ToString();
        }
    }
}
