using HomeBudgetManager.Core;
using Microsoft.Extensions.Primitives; // Importujemy naszą logikę z Core

var builder = WebApplication.CreateBuilder(args);

// 1. Rejestracja serwisów (Dependency Injection)
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RegisterService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// *** ENDPOINT: PANEL GŁÓWNY (DASHBOARD) ***
// Wstrzykujemy IWebHostEnvironment (env), aby wiedzieć, gdzie jest folder wwwroot
app.MapGet("/dashboard", (HttpContext context, IWebHostEnvironment env) => {
    // Sprawdzamy autoryzację
    if (!context.Request.Cookies.ContainsKey("logged_user"))
    {
        return Results.Redirect("/");
    }

    var username = context.Request.Cookies["logged_user"];

    // 1. Ścieżka do pliku HTML
    var filePath = Path.Combine(env.WebRootPath, "dashboard.html");

    // 2. Wczytujemy treść pliku do zmiennej
    // W prawdziwej produkcji warto by to cache'ować, ale dla prostego appa jest ok
    var html = File.ReadAllText(filePath);

    // 3. Podmieniamy nasz placeholder {username} na prawdziwą nazwę
    html = html.Replace("{username}", username);

    return Results.Content(html, "text/html");
});

// *** ENDPOINT LOGOWANIA ***
app.MapPost("/login", (HttpContext httpContext, AuthService authService) => {
    
    var username = httpContext.Request.Form["username"];
    var password = httpContext.Request.Form["password"];

    bool isValid = authService.ValidateUser(username, password);

    if (isValid)
    {
        httpContext.Response.Cookies.Append("logged_user", username);
        // Przekierowanie htmx po udanym logowaniu
        httpContext.Response.Headers.Append("HX-Redirect", "/dashboard");
        return Results.Ok();
    }
    else
    {
        var htmlResponse = "<div class='p-4 bg-red-100 border border-red-400 text-red-700 rounded'>Błąd: Nieprawidłowy login lub hasło.</div>";
        return Results.Content(htmlResponse, "text/html");
    }
});


app.MapGet("/registration", (HttpContext context, IWebHostEnvironment env) => {

    var filePath = Path.Combine(env.WebRootPath, "registration.html");

    var html = File.ReadAllText(filePath);

    return Results.Content(html, "text/html");
});

app.MapGet("/register-form", (HttpContext httpContext) => {

    httpContext.Response.Headers.Append("HX-Redirect", "/registration");
    return Results.Ok();
    
});


app.MapGet("/index", (HttpContext context, IWebHostEnvironment env) => {

    var filePath = Path.Combine(env.WebRootPath, "index.html");

    var html = File.ReadAllText(filePath);

    return Results.Content(html, "text/html");
});


app.MapGet("/login", (HttpContext httpContext) => {

    httpContext.Response.Headers.Append("HX-Redirect", "/index");
    return Results.Ok();

});

app.MapPost("/register", (HttpContext httpContext, RegisterService registerService) => {

    var username = httpContext.Request.Form["username"];
    var password = httpContext.Request.Form["password"];
    var email = httpContext.Request.Form["email"];
    

    if(StringValues.IsNullOrEmpty(username) || StringValues.IsNullOrEmpty(password) || StringValues.IsNullOrEmpty(email))
    {
        var htmlResponse = "<div class='p-4 bg-red-100 border border-red-400 text-red-700 rounded'>Błąd: Nie podano wszystkich danych!</div>";
        return Results.Content(htmlResponse, "text/html"); 
    }
    bool isRegistered = registerService.isRegistered(username);

    if (!isRegistered)
    {
        registerService.registerUser(email, username, password);
        var htmlResponse = "<div class='p-4 bg-red-100 border border-red-400 text-red-700 rounded'>Rejestracja powiodła się</div>";
        return Results.Content(htmlResponse, "text/html");
    }
    else
    {
        var htmlResponse = "<div class='p-4 bg-red-100 border border-red-400 text-red-700 rounded'>Błąd: Już istnieje taki login!</div>";
        return Results.Content(htmlResponse, "text/html");
    }
});

app.Run();