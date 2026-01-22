using HomeBudgetManager.Core;
using HomeBudgetManager.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives; // Importujemy naszą logikę z Core

var builder = WebApplication.CreateBuilder(args);

// 1. Rejestracja serwisów (Dependency Injection)
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RegisterService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<ChartService>();
builder.Services.AddScoped<ReportService>();

// QuestPDF License
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
});

// DB fragment
var connectionStringAzure = builder.Configuration.GetConnectionString("AzureConnection");
var connectionStringLocal = builder.Configuration.GetConnectionString("HbmDatabase");

// builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionStringLocal, b => b.MigrationsAssembly("HomeBudgetManager.Core")));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionStringLocal, b =>
        b.MigrationsAssembly("HomeBudgetManager.Core")));


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Pobieramy serwis z kontenera DI
    var categoryService = scope.ServiceProvider.GetRequiredService<CategoryService>();
    categoryService.addDefaultCategories();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UpdateDatabase();
app.MapAllEndpoints();

app.Run();