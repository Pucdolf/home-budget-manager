using HomeBudgetManager.Core;
using Microsoft.Extensions.Primitives;

namespace HomeBudgetManager.Web.appMaps
{
    public class LoginEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/login", (HttpContext httpContext, AuthService authService) => {

                var email = httpContext.Request.Form["email"];
                var password = httpContext.Request.Form["password"];

                bool isValid = authService.ValidateUserByEmail(email, password);

                if (isValid)
                {
                    var user = authService.GetUserByEmail(email);
                    httpContext.Response.Cookies.Append("logged_user", user.Login); // Still store username/login for other services
                    httpContext.Response.Cookies.Append("user_id", user.Id.ToString());
                    // redirect after succesfull login
                    httpContext.Response.Headers.Append("HX-Redirect", "/dashboard");
                    return Results.Ok();
                }
                else
                {
                    var htmlResponse = "<div class='p-4 bg-red-100 border border-red-400 text-red-700 rounded'>Błąd: Nieprawidłowy email lub hasło.</div>";
                    return Results.Content(htmlResponse, "text/html");
                }
            });

            app.MapGet("/login", (HttpContext httpContext) => {

                httpContext.Response.Headers.Append("HX-Redirect", "/index");
                return Results.Ok();

            });
        }
    }
}
