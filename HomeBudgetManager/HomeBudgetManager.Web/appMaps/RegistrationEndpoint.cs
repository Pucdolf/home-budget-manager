using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HomeBudgetManager.Core;
using HomeBudgetManager.Core.DBTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace HomeBudgetManager.Web.appMaps
{
    public class RegistrationEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/registration", (HttpContext context, IWebHostEnvironment env) => {

                var filePath = Path.Combine(env.WebRootPath, "registration.html");

                return Results.Content(File.ReadAllText(filePath), "text/html");
            });

            app.MapPost("/registration", (HttpContext httpContext, RegisterService registerService) => {

                var username = httpContext.Request.Form["username"];
                var password = httpContext.Request.Form["password"];
                var email = httpContext.Request.Form["email"];

                if (StringValues.IsNullOrEmpty(username) || StringValues.IsNullOrEmpty(password) || StringValues.IsNullOrEmpty(email))
                {
                    var htmlResponse = "<div class='p-4 bg-red-100 border border-red-400 text-red-700 rounded'>Błąd: Nie podano wszystkich danych!</div>";
                    return Results.Content(htmlResponse, "text/html");
                }

                // check if email is taken
                if (registerService.IsEmailTaken(email))
                {
                    var htmlResponse = "<div class='p-4 bg-red-100 border border-red-400 text-red-700 rounded'>Błąd: Ten adres e-mail jest już zajęty!</div>";
                    return Results.Content(htmlResponse, "text/html");
                }

                // register user
                registerService.RegisterUser(email, username, password);
                var successResponse = @"
                    <div class='p-4 bg-green-100 border border-green-400 text-green-700 rounded'>
                        Rejestracja powiodła się!
                    </div>
                    <script>
                        setTimeout(function() {
                            window.location.href = '/index.html';
                        }, 1000);
                    </script>";
                return Results.Content(successResponse, "text/html");
            });
        }
    }
}
