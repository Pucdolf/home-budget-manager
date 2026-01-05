using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Web;
using HomeBudgetManager.Web.Endpoints;
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


// Dodawanie grupy endpointów dashboardu z prefiksem /dashboard
var DashboardGroup =app.MapGroup("/dashboard").MapDashboardEndpoints();

app.MapAllEndpoints();


// *** ENDPOINT: PANEL GŁÓWNY (DASHBOARD) ***
// Wstrzykujemy IWebHostEnvironment (env), aby wiedzieć, gdzie jest folder wwwroot
//app.MapGet("/dashboard", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
//    {
//        // Sprawdzamy autoryzację
//        if (!context.Request.Cookies.ContainsKey("logged_user"))
//        {
//            return Results.Redirect("/");
//        }

//        var username = context.Request.Cookies["logged_user"];

//        var user = await db.Users.FirstOrDefaultAsync(u => u.user_login == username);

//        if (user == null)
//        {
//            return Results.Redirect("/");
//        }

//        // Pobierz ostatnie transakcje. W take() decydujemy ile transakcji wyświetlić
//        var transactions = await db.Transactions
//            .Where(t => t.DBUserId == user.user_id)
//            .OrderByDescending(t => t.transaction_date)
//            .Take(10)
//            .ToListAsync();

//        var balance = await db.Transactions
//                            .Where(t => t.DBUserId == user.user_id)
//                            .SumAsync(t => t.transaction_value);

//        // Generuj HTML dla transakcji
//        var transactionsHtml = string.Join("", transactions.Select(t => 
//        $@"
//            <li class='transaction-item'>
//                <div class='transaction-main'>
//                    <span class='transaction-amount'>{(t.transaction_value >= 0 ? "+ " : "- ")}{Math.Abs(t.transaction_value):N2} zł</span>
//                    <span class='transaction-category'>{t.transaction_category}</span>
//                    <span class='transaction-date'>{t.transaction_date:yyyy-MM-dd}</span>
//                </div>
//                <div class='transaction-actions'>
//                    <button class='btn-secondary' onclick='editTransaction({t.transaction_id})'>Edytuj</button>
//                    <button class='btn-danger' hx-delete='/dashboard/transactions/{t.transaction_id}' hx-confirm='Czy na pewno chcesz usunąć tę transakcję?'>Usuń</button>
//                </div>
//            </li>
//        "));

//        // 1. Ścieżka do pliku HTML
//        var filePath = Path.Combine(env.WebRootPath, "dashboard.html");

//        // 2. Wczytujemy treść pliku do zmiennej
//        // W prawdziwej produkcji warto by to cache'ować, ale dla prostego appa jest ok
//        var html = File.ReadAllText(filePath);

//        // 3. Podmieniamy nasz placeholder {username} na prawdziwą nazwę
//        html = html.Replace("{username}", username);
//        html = html.Replace("{balance}", balance.ToString("N2"));
//        html = html.Replace("{transactions}", transactionsHtml);

//        return Results.Content(html, "text/html");
//    });

//app.MapGet("/add-transaction", async (HttpContext context, IWebHostEnvironment env, AppDbContext db) =>
//{
//    if (!context.Request.Cookies.ContainsKey("logged_user"))
//    {
//        return Results.Redirect("/");
//    }

//    var userId = int.Parse(context.Request.Cookies["user_id"]);
//    var user = await db.Users.FirstOrDefaultAsync(u => u.user_id == userId);
//    var username = context.Request.Cookies["logged_user"];
    
//    // Wczytaj plik HTML z kodowaniem UTF-8
//    var filePath = Path.Combine(env.WebRootPath, "addTransaction.html");
//    var html = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

//    html = html.Replace("{username}", user.user_login);

//    return Results.Content(html, "text/html; charset=utf-8"); 
//});


// *** ENDPOINT LOGOWANIA ***
//app.MapPost("/login", (HttpContext httpContext, AuthService authService) => {
    
//    var username = httpContext.Request.Form["username"];
//    var password = httpContext.Request.Form["password"];

//    bool isValid = authService.ValidateUser(username, password);

//    if (isValid)
//    {
//        var user = authService.GetUserByUsername(username);
//        httpContext.Response.Cookies.Append("logged_user", username);
//        httpContext.Response.Cookies.Append("user_id", user.user_id.ToString());
//        // Przekierowanie htmx po udanym logowaniu
//        httpContext.Response.Headers.Append("HX-Redirect", "/dashboard");
//        return Results.Ok();
//    }
//    else
//    {
//        var htmlResponse = "<div class='p-4 bg-red-100 border border-red-400 text-red-700 rounded'>Błąd: Nieprawidłowy login lub hasło.</div>";
//        return Results.Content(htmlResponse, "text/html");
//    }
//});

//app.MapPost("/logout", (HttpContext context) =>
//{
//    context.Response.Cookies.Delete("logged_user");
//    return Results.Redirect("/index.html");
//});

//app.MapGet("/registration", (HttpContext context, IWebHostEnvironment env) => {

//    var filePath = Path.Combine(env.WebRootPath, "registration.html");

//    var html = File.ReadAllText(filePath);

//    return Results.Content(html, "text/html");
//});

//app.MapGet("/register-form", (HttpContext httpContext) => {

//    httpContext.Response.Headers.Append("HX-Redirect", "/registration");
//    return Results.Ok();
    
//});


//app.MapGet("/index", (HttpContext context, IWebHostEnvironment env) => {

//    var filePath = Path.Combine(env.WebRootPath, "index.html");

//    var html = File.ReadAllText(filePath);

//    return Results.Content(html, "text/html");
//});


//app.MapGet("/login", (HttpContext httpContext) => {

//    httpContext.Response.Headers.Append("HX-Redirect", "/index");
//    return Results.Ok();

//});
//app.MapPost("/register", (HttpContext httpContext, RegisterService registerService) => {

//    var username = httpContext.Request.Form["username"];
//    var password = httpContext.Request.Form["password"];
//    var email = httpContext.Request.Form["email"];

//    if (StringValues.IsNullOrEmpty(username) || StringValues.IsNullOrEmpty(password) || StringValues.IsNullOrEmpty(email))
//    {
//        var htmlResponse = "<div class='p-4 bg-red-100 border border-red-400 text-red-700 rounded'>Błąd: Nie podano wszystkich danych!</div>";
//        return Results.Content(htmlResponse, "text/html");
//    }

//    // Sprawdź, czy login lub email są już zajęte
//    if (registerService.IsUsernameTaken(username))
//    {
//        var htmlResponse = "<div class='p-4 bg-red-100 border border-red-400 text-red-700 rounded'>Błąd: Ten login jest już zajęty!</div>";
//        return Results.Content(htmlResponse, "text/html");
//    }

//    if (registerService.IsEmailTaken(email))
//    {
//        var htmlResponse = "<div class='p-4 bg-red-100 border border-red-400 text-red-700 rounded'>Błąd: Ten adres e-mail jest już zajęty!</div>";
//        return Results.Content(htmlResponse, "text/html");
//    }

//    // Zarejestruj użytkownika
//    registerService.RegisterUser(email, username, password);
//    var successResponse = "<div class='p-4 bg-green-100 border border-green-400 text-green-700 rounded'>Rejestracja powiodła się!</div>";
//    return Results.Content(successResponse, "text/html");
//});


//app.MapPost("/create-household", async (HttpContext context, AppDbContext db) =>
//{
//    var form = context.Request.Form;
//    var name = form["name"];
//    var description = form["description"];
//    var userLogin = context.Request.Cookies["logged_user"];

//    if (string.IsNullOrWhiteSpace(name))
//    {
//        return Results.Content("<div class='error'>Błąd: nazwa grupy jest wymagana.</div>", "text/html");
//    }

//    var user = await db.Users.FirstOrDefaultAsync(u => u.user_login == userLogin);
//    if (user == null)
//    {
//        return Results.Content("<div class='error'>Błąd: użytkownik niezalogowany.</div>", "text/html");
//    }

//    if (user.user_house_id != null)
//    {
//        return Results.Content("<div class='error'>Błąd: użytkownik należy już do domostwa.</div>", "text/html");
//    }

//    // 1. Stwórz domostwo
//    var house = new DBHouse
//    {
//        house_name = name,
//        house_description = description,
//        house_admin_id = user.user_id,
//        house_join_code = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper() // np. "A1B2C3"
//    };
//    db.Houses.Add(house);
//    await db.SaveChangesAsync();

//    // 2. Przypisz użytkownika do domu i ustaw jako admin
//    user.user_house_id = house.house_id;
//    user.user_role = SystemRole.HouseholdAdmin;

//    await db.SaveChangesAsync();

//    return Results.Content("<div class='success'>Domostwo utworzone!</div>", "text/html");
//});

//app.MapPost("/join-household", async (HttpContext context, AppDbContext db) =>
//{
//    var code = context.Request.Form["code"].ToString().ToUpper();
//    var login = context.Request.Cookies["logged_user"];

//    var user = await db.Users.FirstOrDefaultAsync(u => u.user_login == login);
//    if (user == null || user.user_house_id != null)
//    {
//        return Results.Content("<div class='error'>Nie możesz dołączyć do nowego domostwa.</div>", "text/html");
//    }

//    var house = await db.Houses.FirstOrDefaultAsync(h => h.house_join_code == code);
//    if (house == null)
//    {
//        return Results.Content("<div class='error'>Nie znaleziono domostwa o takim kodzie.</div>", "text/html");
//    }

//    user.user_house_id = house.house_id;
//    user.user_role = SystemRole.HouseholdMember;

//    await db.SaveChangesAsync();

//    return Results.Content("<div class='success'>Dołączono do domostwa!</div>", "text/html");
//});


//app.MapGet("/dashboard-household", async (HttpContext context, AppDbContext db) =>
//{
//    var login = context.Request.Cookies["logged_user"];
//    if (string.IsNullOrEmpty(login))
//        return Results.Text("Błąd: użytkownik niezalogowany", "text/plain");

//    var user = await db.Users
//        .Include(u => u.user_house) // załaduj domostwo
//        .FirstOrDefaultAsync(u => u.user_login == login);

//    if (user == null)
//        return Results.Text("Błąd: użytkownik nie istnieje", "text/plain");

//    try
//    {
//        if (user.user_house_id is null)
//        {
//            // użytkownik nie należy do domostwa
//            var html = $@"
//            <section class='card'>
//                <h2>Twoje domostwo</h2>
//                <p>Nie jesteś jeszcze członkiem żadnego domostwa.</p>
//                <div class='actions-box'>
//                    <a href='createHousehold.html' class='btn-primary'>Utwórz domostwo</a>
//                    <a href='joinHousehold.html' class='btn-primary'>Dołącz do domostwa</a>
//                </div>
//            </section>";
//            return Results.Content(html, "text/html");
//        }
//        else
//        {
//            // użytkownik ma domostwo
//            var house = user.user_house!;
//            var html = $@"
//            <section class='card'>
//                <h2>Twoje domostwo</h2>
//                <p><strong>Nazwa:</strong> {house.house_name}</p>
//                <p><strong>Opis:</strong> {house.house_description}</p>
//                <p><strong>Admin ID:</strong> {house.house_admin_id}</p>
//                <p><strong>Kod zaproszenia:</strong> {house.house_join_code}</p>
//                <!-- Tu później dodasz np. listę członków -->
//            </section>";
//            return Results.Content(html, "text/html");
//        }
//    } catch (Exception ex)
//    {
//        Console.WriteLine($"Błąd wczytania strony: {ex.Message}");
//        if (ex.InnerException != null)
//        {
//            Console.WriteLine($"Inner: {ex.InnerException.Message}");
//        }
//        return Results.Content($"<div class='error'>Błąd serwera: nie udało się wczytać strony.</div>", "text/html");
//    }
//});



app.Run();