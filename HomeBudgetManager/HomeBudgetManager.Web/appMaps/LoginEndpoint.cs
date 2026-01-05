using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using HomeBudgetManager.Web.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace HomeBudgetManager.Web.appMaps
{
    public class LoginEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("/login", (HttpContext httpContext, AuthService authService) => {

                var username = httpContext.Request.Form["username"];
                var password = httpContext.Request.Form["password"];

                bool isValid = authService.ValidateUser(username, password);

                if (isValid)
                {
                    var user = authService.GetUserByUsername(username);
                    httpContext.Response.Cookies.Append("logged_user", username);
                    httpContext.Response.Cookies.Append("user_id", user.user_id.ToString());
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

            app.MapGet("/login", (HttpContext httpContext) => {

                httpContext.Response.Headers.Append("HX-Redirect", "/index");
                return Results.Ok();

            });
        }
    }
}
