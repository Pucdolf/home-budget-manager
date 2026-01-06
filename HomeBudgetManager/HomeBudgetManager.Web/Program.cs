using HomeBudgetManager.Core;
using HomeBudgetManager.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives; // Importujemy naszą logikę z Core

var builder = WebApplication.CreateBuilder(args);

// 1. Rejestracja serwisów (Dependency Injection)
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RegisterService>();

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

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapAllEndpoints();

app.Run();